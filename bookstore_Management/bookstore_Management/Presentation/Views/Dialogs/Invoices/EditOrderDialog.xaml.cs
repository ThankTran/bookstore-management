using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using bookstore_Management.DTOs.Order.Requests;
using bookstore_Management.DTOs.Order.Responses;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.Presentation.Views.Dialogs.Invoices
{
    /// <summary>
    /// Dialog sửa hóa đơn bán hàng
    /// Chỉ cho phép sửa: Discount, Notes, PaymentMethod
    /// KHÔNG cho phép sửa: Staff, Customer, OrderDetails (vì ảnh hưởng tồn kho)
    /// </summary>
    public partial class EditOrderDialog : Window
    {
        private readonly IOrderService _orderService;
        private readonly string _orderId;
        private OrderResponseDto _originalData;

        private readonly ObservableCollection<OrderDetailDisplayItem> _orderDetails
            = new ObservableCollection<OrderDetailDisplayItem>();

        public EditOrderDialog(string orderId)
        {
            InitializeComponent();

            _orderId = orderId;

            // Initialize service
            var context = new BookstoreDbContext();
            var unitOfWork = new UnitOfWork(context);

            _orderService = new OrderService(unitOfWork);

            dgOrderDetails.ItemsSource = _orderDetails;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadOrderData();
        }

        private async void LoadOrderData()
        {
            try
            {
                var result = await _orderService.GetOrderByIdAsync(_orderId);

                if (!result.IsSuccess || result.Data == null)
                {
                    MessageBox.Show(result.ErrorMessage ?? "Không tìm thấy hóa đơn!",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    DialogResult = false;
                    Close();
                    return;
                }

                _originalData = result.Data;

                // Fill basic info (READ-ONLY)
                tbOrderId.Text = _originalData.OrderId;
                tbStaffName.Text = _originalData.StaffName;
                tbCustomerName.Text = _originalData.CustomerName ?? "Khách vãng lai";
                tbCreatedDate.Text = _originalData.CreatedDate.ToString("dd/MM/yyyy HH:mm");

                // Fill EDITABLE fields
                cbPaymentMethod.SelectedIndex = (int)_originalData.PaymentMethod;
                tbNotes.Text = _originalData.Notes;

                // Calculate original discount VND from rate
                var subtotal = _originalData.OrderDetails?.Sum(x => x.Subtotal) ?? 0;
                var discountVnd = subtotal * _originalData.Discount;
                tbDiscount.Text = ((long)discountVnd).ToString();

                // Load details (READ-ONLY display)
                if (_originalData.OrderDetails != null)
                {
                    foreach (var detail in _originalData.OrderDetails)
                    {
                        _orderDetails.Add(new OrderDetailDisplayItem
                        {
                            BookId = detail.BookId,
                            BookName = detail.BookName,
                            Quantity = detail.Quantity,
                            SalePrice = detail.SalePrice,
                            Subtotal = detail.Subtotal
                        });
                    }
                }

                UpdateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
                Close();
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn cập nhật hóa đơn {_orderId}?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                var subtotal = _orderDetails.Sum(x => x.Subtotal);
                var discountVnd = ParseDecimalSafe(tbDiscount.Text);
                if (discountVnd < 0) discountVnd = 0;
                if (discountVnd > subtotal) discountVnd = subtotal;

                var discountRate = subtotal <= 0 ? 0 : (discountVnd / subtotal);

                var updateDto = new UpdateOrderRequestDto
                {
                    PaymentMethod = (PaymentType)cbPaymentMethod.SelectedIndex,
                    Discount = discountRate,
                    Notes = tbNotes.Text?.Trim()
                };

                var result = await _orderService.UpdateOrderAsync(_orderId, updateDto);

                if (result.IsSuccess)
                {
                    MessageBox.Show("Cập nhật hóa đơn thành công!", "Thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show(result.ErrorMessage, "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            var confirm = MessageBox.Show(
                "Bạn có chắc muốn hủy? Các thay đổi sẽ không được lưu.",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (confirm == MessageBoxResult.Yes)
            {
                DialogResult = false;
                Close();
            }
        }

        private void Discount_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTotals();
        }

        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void UpdateTotals()
        {
            var subtotal = _orderDetails.Sum(x => x.Subtotal);
            var discountVnd = ParseDecimalSafe(tbDiscount?.Text);

            if (discountVnd < 0) discountVnd = 0;
            if (discountVnd > subtotal) discountVnd = subtotal;

            tbSubtotal.Text = $"{subtotal:N0} ₫";
            tbTotal.Text = $"{(subtotal - discountVnd):N0} ₫";

            // Auto-correct discount if exceeds subtotal
            if (tbDiscount != null)
            {
                var currentDiscount = ParseDecimalSafe(tbDiscount.Text);
                if (currentDiscount != discountVnd)
                {
                    tbDiscount.TextChanged -= Discount_TextChanged;
                    tbDiscount.Text = ((long)discountVnd).ToString();
                    tbDiscount.TextChanged += Discount_TextChanged;
                }
            }
        }

        private bool ValidateForm()
        {
            var discountVnd = ParseDecimalSafe(tbDiscount.Text);
            var subtotal = _orderDetails.Sum(x => x.Subtotal);

            if (discountVnd < 0)
            {
                MessageBox.Show("Giảm giá không được âm!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbDiscount.Focus();
                return false;
            }

            if (discountVnd > subtotal)
            {
                MessageBox.Show("Giảm giá không được lớn hơn tổng tiền hàng!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbDiscount.Focus();
                return false;
            }

            return true;
        }

        private static decimal ParseDecimalSafe(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0m;
            text = text.Trim().Replace(",", "");
            return decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var v) ? v : 0m;
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            BtnCancel_Click(sender, e);
        }
    }

    public class OrderDetailDisplayItem
    {
        public string BookId { get; set; }
        public string BookName { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}