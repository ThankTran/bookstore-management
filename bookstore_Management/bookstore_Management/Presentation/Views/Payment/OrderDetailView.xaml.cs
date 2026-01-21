using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.DTOs.Order.Responses;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Presentation.Views;
using bookstore_Management.Presentation.Views.Dialogs.Share;
using bookstore_Management.Core.Enums;
using bookstore_Management.Presentation.ViewModels;
using bookstore_Management.Presentation.Views.Dialogs.Invoices;
using bookstore_Management.Presentation.Views.Orders;
using Microsoft.Extensions.DependencyInjection;

namespace bookstore_Management.Presentation.Views.Payment
{
    /// <summary>
    /// Interaction logic for OrderDetailView.xaml
    /// </summary>
    public partial class OrderDetailView : UserControl, IDisposable
    {
        private readonly IOrderService _orderService;
        private string _currentOrderId;
        private OrderResponseDto _currentOrder;


        public OrderDetailView(IOrderService orderService)
        {
            InitializeComponent();
            _orderService = orderService;
        }

        public void LoadOrderAsync(string orderId)
        {
            if (string.IsNullOrEmpty(orderId)) return;

            _currentOrderId = orderId;
            var result = _orderService.GetOrderById(orderId);

            if (!result.IsSuccess || result.Data == null)
            {
                MessageBox.Show("Không thể tải thông tin đơn hàng: " + result.ErrorMessage,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var order = result.Data;
            _currentOrder = order;

            // Load details
            var detailsResult = _orderService.GetOrderDetails(orderId);
            if (detailsResult.IsSuccess && detailsResult.Data != null)
            {
                var details = detailsResult.Data.Select((d, index) => new
                {
                    Index = index + 1,
                    d.BookId,
                    d.BookName,
                    d.Quantity,
                    d.SalePrice,
                    d.Subtotal
                }).ToList();

                dgOrderDetails.ItemsSource = details;

                // Calculate summary
                var totalItems = details.Count;
                var totalQuantity = details.Sum(d => d.Quantity);
                txtTotalItems.Text = $"{totalItems} mặt hàng";
                txtTotalQuantity.Text = $"{totalQuantity} cuốn";
            }

            // Set header info
            txtOrderId.Text = $"Mã đơn: {order.OrderId}";
            txtInfoOrderId.Text = order.OrderId;
            txtCustomer.Text = order.CustomerName ?? "Khách vãng lai";
            txtInfoCustomer.Text = order.CustomerName ?? "Khách vãng lai";
            txtStatus.Text = "Hoàn thành";
            txtTotalAmount.Text = order.TotalPrice.ToString("N0") + " ₫";
            txtSubtotal.Text = (order.TotalPrice + order.Discount).ToString("N0") + " ₫";
            txtDiscount.Text = order.Discount > 0 ? "- " + order.Discount.ToString("N0") + " ₫" : "0 ₫";
            txtGrandTotal.Text = order.TotalPrice.ToString("N0") + " ₫";
            txtStaffName.Text = order.StaffName ?? "N/A";
            txtCreatedDate.Text = order.CreatedDate.ToString("dd/MM/yyyy - HH:mm");
            txtNotes.Text = order.Notes ?? "Không có ghi chú";

            // Payment method
            string paymentMethodText;
            switch (order.PaymentMethod)
            {
                case PaymentType.Cash:
                    paymentMethodText = " Tiền mặt";
                    break;
                case PaymentType.Card:
                    paymentMethodText = " Thẻ";
                    break;
                case PaymentType.BankTransfer:
                    paymentMethodText = " Chuyển khoản";
                    break;
                case PaymentType.DebitCard:
                    paymentMethodText = "Tín dụng";
                    break;
                default:
                    paymentMethodText = " Tiền mặt";
                    break;
            }

            txtPaymentMethod.Text = paymentMethodText;
            txtInfoPaymentMethod.Text = paymentMethodText;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow == null) return;

            mainWindow.MainFrame.Content =
                App.Services.GetRequiredService<InvoiceView>();

        }

        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentOrderId))
            {
                MessageBox.Show("Không có thông tin đơn hàng để in.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (string.IsNullOrEmpty(_currentOrderId))
            {
                MessageBox.Show("Không có thông tin phiếu nhập để in.", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var print = new PrintInvoice(
                _currentOrder.OrderId,
                InvoiceType.Export,
                _currentOrder
            );

            print.ShowDialog();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentOrderId))
            {
                MessageBox.Show("Không có thông tin phiếu nhập để xóa.",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            bool confirmed = Delete.ShowForInvoice(
                _currentOrder.OrderId,
                Window.GetWindow(this)
            );

            if (!confirmed) return;

            var result = _orderService.DeleteOrder(_currentOrderId);
            if (!result.IsSuccess)
            {
                MessageBox.Show($"Không thể xóa phiếu nhập.\nChi tiết lỗi: {result.ErrorMessage}",
                    "Lỗi xóa dữ liệu",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Đã xóa phiếu nhập thành công.",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow == null) return;

            mainWindow.MainFrame.Content =
                App.Services.GetRequiredService<InvoiceView>();
        }

        #region IDisposable

        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // // Dispose managed resources
                    // _unitOfWork?.Dispose();
                    // context?.Dispose();
                }
                _disposed = true;
            }
        }

        #endregion
    }
}