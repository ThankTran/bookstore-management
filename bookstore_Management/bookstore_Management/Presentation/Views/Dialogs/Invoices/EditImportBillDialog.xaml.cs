using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using bookstore_Management.DTOs.ImportBill.Requests;
using bookstore_Management.DTOs.ImportBill.Responses;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;

namespace bookstore_Management.Presentation.Views.Dialogs.Invoices
{
    /// <summary>
    /// Dialog sửa phiếu nhập hàng
    /// Tái sử dụng UI tương tự CreateImportBill nhưng có sẵn dữ liệu
    /// </summary>
    public partial class EditImportBillDialog : Window
    {
        private readonly IImportBillService _importBillService;
        private readonly string _importBillId;
        private readonly ObservableCollection<ImportBookItem> _bookItems = new ObservableCollection<ImportBookItem>();

        public EditImportBillDialog(string importBillId,
            IImportBillService importBillService)
        {
            InitializeComponent();

            _importBillId = importBillId;
            _importBillService = importBillService;

            icBooks.ItemsSource = _bookItems;
            _bookItems.CollectionChanged += (_, __) => UpdateTotalAmount();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadImportBillData();
        }

        private void LoadImportBillData()
        {
            try
            {
                var result = _importBillService.GetImportBillById(_importBillId);

                if (!result.IsSuccess || result.Data == null)
                {
                    MessageBox.Show(result.ErrorMessage ?? "Không tìm thấy phiếu nhập!",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    DialogResult = false;
                    Close();
                    return;
                }

                var data = result.Data;

                // Fill basic info
                tbImportId.Text = data.Id;
                tbCreatedBy.Text = data.CreatedBy;
                tbCreatedDate.Text = data.CreatedDate.ToString("dd/MM/yyyy HH:mm");
                tbNotes.Text = data.Notes;

                // Set publisher (assuming cbPublisher is ComboBox with Publishers)
                // cbPublisher.SelectedValue = data.PublisherId;

                // Load details
                if (data.ImportBillDetails != null)
                {
                    foreach (var detail in data.ImportBillDetails)
                    {
                        var item = new ImportBookItem
                        {
                            BookId = detail.BookId,
                            BookName = detail.BookName,
                            Quantity = detail.Quantity,
                            ImportPrice = detail.ImportPrice
                        };

                        AttachItemEvents(item);
                        _bookItems.Add(item);
                    }
                }

                UpdateTotalAmount();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
                Close();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn cập nhật phiếu nhập {_importBillId}?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                // Update basic info (Notes only, Publisher cannot change)
                var updateDto = new UpdateImportBillRequestDto
                {
                    Notes = tbNotes.Text?.Trim()
                };

                var basicResult = _importBillService.UpdateImportBill(_importBillId, updateDto);

                if (!basicResult.IsSuccess)
                {
                    MessageBox.Show(basicResult.ErrorMessage, "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Update details (Quantity & Price for each item)
                foreach (var item in _bookItems)
                {
                    var detailResult = _importBillService.UpdateImportItem(
                        _importBillId,
                        item.BookId,
                        item.Quantity,
                        item.ImportPrice
                    );

                    if (!detailResult.IsSuccess)
                    {
                        MessageBox.Show(
                            $"Lỗi cập nhật sách {item.BookName}: {detailResult.ErrorMessage}",
                            "Lỗi",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                        return;
                    }
                }

                MessageBox.Show("Cập nhật phiếu nhập thành công!", "Thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
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

        private void BtnRemoveBook_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ImportBookItem item)
            {
                var confirm = MessageBox.Show(
                    $"Bạn có chắc muốn xóa sách '{item.BookName}' khỏi phiếu nhập?",
                    "Xác nhận",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (confirm != MessageBoxResult.Yes) return;

                try
                {
                    var result = _importBillService.RemoveImportItem(_importBillId, item.BookId);

                    if (result.IsSuccess)
                    {
                        DetachItemEvents(item);
                        _bookItems.Remove(item);
                        UpdateTotalAmount();
                    }
                    else
                    {
                        MessageBox.Show(result.ErrorMessage, "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void AttachItemEvents(ImportBookItem item)
        {
            item.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(ImportBookItem.Quantity) ||
                    e.PropertyName == nameof(ImportBookItem.ImportPrice))
                {
                    UpdateTotalAmount();
                }
            };
        }

        private void DetachItemEvents(ImportBookItem item)
        {
            // In real implementation, store event handler reference
        }

        private void UpdateTotalAmount()
        {
            var total = _bookItems.Sum(x => x.Subtotal);
            tbTotalAmount.Text = $"{total:N0} ₫";
        }

        private bool ValidateForm()
        {
            if (!_bookItems.Any())
            {
                MessageBox.Show("Phiếu nhập phải có ít nhất một sách!",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            foreach (var item in _bookItems)
            {
                if (item.Quantity <= 0)
                {
                    MessageBox.Show($"Số lượng sách '{item.BookName}' phải lớn hơn 0!",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (item.ImportPrice <= 0)
                {
                    MessageBox.Show($"Giá nhập sách '{item.BookName}' phải lớn hơn 0!",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
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
}