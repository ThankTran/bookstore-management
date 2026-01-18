using bookstore_Management.DTOs.Order.Responses;
using bookstore_Management.Services.Interfaces;
using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace bookstore_Management.Presentation.Views.Dialogs.Payment
{
    /// <summary>
    /// Interaction logic for ExportExcelPayment.xaml
    /// </summary>
    public partial class ExportExcelPayment : Window
    {
        private readonly IOrderService _orderService;
        private readonly string _orderId;
        private OrderResponseDto _orderData;

        public ExportExcelPayment(IOrderService orderService, string orderId)
        {
            InitializeComponent();
            _orderService = orderService;
            _orderId = orderId;

            LoadOrderData();
        }

        #region Load Data

        private void LoadOrderData()
        {
            try
            {
                var result = _orderService.GetOrderById(_orderId);

                if (!result.IsSuccess || result.Data == null)
                {
                    MessageBox.Show(result.ErrorMessage ?? "Không tìm thấy đơn hàng!",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                _orderData = result.Data;

                // Update UI
                txtOrderId.Text = _orderData.OrderId;
                txtTotal.Text = $"{_orderData.TotalPrice:N0} ₫";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        #endregion

        #region Event Handlers

        private void chkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            SetAllCheckboxes(true);
        }

        private void chkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            SetAllCheckboxes(false);
        }

        private void cbFileFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Có thể thêm logic tùy chỉnh theo format
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!HasSelectedColumns())
                {
                    MessageBox.Show("Vui lòng chọn ít nhất 1 thông tin để xuất!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_orderData == null)
                {
                    MessageBox.Show("Không có dữ liệu để xuất!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedFormat = ((ComboBoxItem)cbFileFormat.SelectedItem).Tag.ToString();
                string fileName = $"HoaDon_{_orderId}_{DateTime.Now:yyyyMMdd_HHmmss}";
                string defaultPath = System.IO.Path.Combine(
                                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                                    "Downloads");


                switch (selectedFormat)
                {
                    case "xlsx":
                        ExportToExcel(defaultPath, fileName);
                        break;
                    case "csv":
                        ExportToCsv(defaultPath, fileName);
                        break;
                    case "pdf":
                        ExportToPdf(defaultPath, fileName);
                        break;
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất file: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Export Methods

        private void ExportToExcel(string path, string fileName)
        {
            var saveDialog = new SaveFileDialog
            {
                InitialDirectory = path,
                FileName = fileName,
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                DefaultExt = "xlsx"
            };

            if (saveDialog.ShowDialog() != true)
                return;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Hóa đơn");

                int currentRow = 1;

                // Header - Store Info
                worksheet.Cell(currentRow, 1).Value = "BOOKSTORE MANAGEMENT";
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 1).Style.Font.FontSize = 16;
                currentRow++;

                worksheet.Cell(currentRow++, 1).Value = "Địa chỉ: 123 Đường ABC, Quận XYZ";
                worksheet.Cell(currentRow++, 1).Value = "Điện thoại: 0123-456-789";
                currentRow++;

                // Title
                worksheet.Cell(currentRow, 1).Value = "HÓA ĐƠN BÁN HÀNG";
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
                currentRow += 2;

                // Order Info
                if (chkOrderId.IsChecked == true)
                {
                    worksheet.Cell(currentRow, 1).Value = "Mã đơn hàng:";
                    worksheet.Cell(currentRow++, 2).Value = _orderData.OrderId;
                }

                if (chkOrderDate.IsChecked == true)
                {
                    worksheet.Cell(currentRow, 1).Value = "Ngày:";
                    worksheet.Cell(currentRow++, 2).Value = _orderData.CreatedDate.ToString("dd/MM/yyyy HH:mm");
                }

                if (chkStaffName.IsChecked == true)
                {
                    worksheet.Cell(currentRow, 1).Value = "Nhân viên:";
                    worksheet.Cell(currentRow++, 2).Value = _orderData.StaffName ?? "N/A";
                }

                if (chkCustomerName.IsChecked == true)
                {
                    worksheet.Cell(currentRow, 1).Value = "Khách hàng:";
                    worksheet.Cell(currentRow++, 2).Value = _orderData.CustomerName ?? "Khách vãng lai";
                }

                if (chkPaymentMethod.IsChecked == true)
                {
                    worksheet.Cell(currentRow, 1).Value = "Phương thức:";
                    worksheet.Cell(currentRow++, 2).Value = _orderData.PaymentMethod.ToString();
                }

                currentRow++;

                // Product Details
                if (chkOrderDetails.IsChecked == true && _orderData.OrderDetails != null && _orderData.OrderDetails.Any())
                {
                    worksheet.Cell(currentRow, 1).Value = "STT";
                    worksheet.Cell(currentRow, 2).Value = "Tên sản phẩm";
                    worksheet.Cell(currentRow, 3).Value = "Số lượng";
                    worksheet.Cell(currentRow, 4).Value = "Đơn giá";
                    worksheet.Cell(currentRow, 5).Value = "Thành tiền";

                    worksheet.Range(currentRow, 1, currentRow, 5).Style.Font.Bold = true;
                    worksheet.Range(currentRow, 1, currentRow, 5).Style.Fill.BackgroundColor = XLColor.LightGray;
                    currentRow++;

                    int stt = 1;
                    foreach (var detail in _orderData.OrderDetails)
                    {
                        worksheet.Cell(currentRow, 1).Value = stt++;
                        worksheet.Cell(currentRow, 2).Value = detail.BookName;
                        worksheet.Cell(currentRow, 3).Value = detail.Quantity;
                        worksheet.Cell(currentRow, 4).Value = detail.SalePrice;
                        worksheet.Cell(currentRow, 5).Value = detail.Subtotal;
                        currentRow++;
                    }

                    currentRow++;
                }

                // Summary
                decimal subtotal = _orderData.TotalPrice / (1 - _orderData.Discount);

                worksheet.Cell(currentRow, 4).Value = "Tạm tính:";
                worksheet.Cell(currentRow++, 5).Value = subtotal;

                if (chkDiscount.IsChecked == true)
                {
                    worksheet.Cell(currentRow, 4).Value = "Giảm giá:";
                    worksheet.Cell(currentRow++, 5).Value = $"{_orderData.Discount * 100}%";
                }

                if (chkTotalPrice.IsChecked == true)
                {
                    worksheet.Cell(currentRow, 4).Value = "Tổng cộng:";
                    worksheet.Cell(currentRow, 4).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 5).Value = _orderData.TotalPrice;
                    worksheet.Cell(currentRow, 5).Style.Font.Bold = true;
                    currentRow++;
                }

                if (chkNotes.IsChecked == true && !string.IsNullOrWhiteSpace(_orderData.Notes))
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = "Ghi chú:";
                    worksheet.Cell(currentRow, 2).Value = _orderData.Notes;
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(saveDialog.FileName);

                MessageBox.Show($"Xuất file thành công!\nĐường dẫn: {saveDialog.FileName}",
                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportToCsv(string path, string fileName)
        {
            var saveDialog = new SaveFileDialog
            {
                InitialDirectory = path,
                FileName = fileName,
                Filter = "CSV Files (*.csv)|*.csv",
                DefaultExt = "csv"
            };

            if (saveDialog.ShowDialog() != true)
                return;

            using (var writer = new StreamWriter(saveDialog.FileName, false, System.Text.Encoding.UTF8))
            {
                // Header
                writer.WriteLine("BOOKSTORE MANAGEMENT");
                writer.WriteLine("HÓA ĐƠN BÁN HÀNG");
                writer.WriteLine();

                // Order Info
                if (chkOrderId.IsChecked == true)
                    writer.WriteLine($"Mã đơn hàng,{_orderData.OrderId}");

                if (chkOrderDate.IsChecked == true)
                    writer.WriteLine($"Ngày,{_orderData.CreatedDate:dd/MM/yyyy HH:mm}");

                if (chkStaffName.IsChecked == true)
                    writer.WriteLine($"Nhân viên,{_orderData.StaffName ?? "N/A"}");

                if (chkCustomerName.IsChecked == true)
                    writer.WriteLine($"Khách hàng,{_orderData.CustomerName ?? "Khách vãng lai"}");

                if (chkPaymentMethod.IsChecked == true)
                    writer.WriteLine($"Phương thức,{_orderData.PaymentMethod}");

                writer.WriteLine();

                // Product Details
                if (chkOrderDetails.IsChecked == true && _orderData.OrderDetails != null)
                {
                    writer.WriteLine("STT,Tên sản phẩm,Số lượng,Đơn giá,Thành tiền");

                    int stt = 1;
                    foreach (var detail in _orderData.OrderDetails)
                    {
                        writer.WriteLine($"{stt++},\"{detail.BookName}\",{detail.Quantity},{detail.SalePrice},{detail.Subtotal}");
                    }

                    writer.WriteLine();
                }

                // Summary
                if (chkTotalPrice.IsChecked == true)
                {
                    writer.WriteLine($"Tổng cộng,{_orderData.TotalPrice}");
                }

                if (chkNotes.IsChecked == true && !string.IsNullOrWhiteSpace(_orderData.Notes))
                {
                    writer.WriteLine($"Ghi chú,\"{_orderData.Notes}\"");
                }
            }

            MessageBox.Show($"Xuất file thành công!\nĐường dẫn: {saveDialog.FileName}",
                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportToPdf(string path, string fileName)
        {
            MessageBox.Show("Chức năng xuất PDF đang được phát triển!",
                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Helper Methods

        private void SetAllCheckboxes(bool isChecked)
        {
            foreach (var child in pnlColumns.Children)
            {
                if (child is CheckBox checkbox)
                {
                    checkbox.IsChecked = isChecked;
                }
            }
        }

        private bool HasSelectedColumns()
        {
            foreach (var child in pnlColumns.Children)
            {
                if (child is CheckBox checkbox && checkbox.IsChecked == true)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
