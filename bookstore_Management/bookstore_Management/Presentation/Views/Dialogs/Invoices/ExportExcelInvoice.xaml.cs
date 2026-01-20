using bookstore_Management.DTOs.ImportBill.Responses;
using bookstore_Management.DTOs.Order.Responses;
using bookstore_Management.Services.Interfaces;
using Microsoft.Win32;
using RBush;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace bookstore_Management.Presentation.Views.Dialogs.Invoices
{
    public partial class ExportExcelInvoice : Window
    {
        private readonly IImportBillService _importBillService;
        private readonly IOrderService _orderService;

        private List<ImportBillResponseDto> _allImports;
        private List<OrderResponseDto> _allOrders;

        public ExportExcelInvoice(
            IImportBillService importBillService,
            IOrderService orderService)
        {
            InitializeComponent();

            _importBillService = importBillService;
            _orderService = orderService;

            LoadData();
            UpdateStatistics();
        }

        #region Data Loading
        private async Task LoadData()
        {
            try
            {
                // Load all imports
                var importResult = await _importBillService.GetAllImportBillsAsync();
                _allImports = importResult.IsSuccess && importResult.Data != null
                    ? importResult.Data.ToList()
                    : new List<ImportBillResponseDto>();

                // Load all orders
                var orderResult = await _orderService.GetAllOrdersAsync();
                _allOrders = orderResult.IsSuccess && orderResult.Data != null
                    ? orderResult.Data.ToList()
                    : new List<OrderResponseDto>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Event Handlers
        private void DateRange_Changed(object sender, RoutedEventArgs e)
        {
            if (rbCustomRange?.IsChecked == true)
            {
                customDatePanel.Visibility = Visibility.Visible;

                if (dpStartDate.SelectedDate == null)
                    dpStartDate.SelectedDate = DateTime.Now.AddMonths(-1);

                if (dpEndDate.SelectedDate == null)
                    dpEndDate.SelectedDate = DateTime.Now;
            }
            else
            {
                customDatePanel.Visibility = Visibility.Collapsed;
            }

            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            if (_allImports == null || _allOrders == null) return;

            var (imports, orders) = GetFilteredData();

            tbImportCount.Text = imports.Count.ToString();
            tbExportCount.Text = orders.Count.ToString();
            tbTotalCount.Text = (imports.Count + orders.Count).ToString();

            UpdateDateRangeDisplay();
        }

        private void UpdateDateRangeDisplay()
        {
            if (rbAllTime?.IsChecked == true)
            {
                tbDateRange.Text = "Tất cả thời gian";
            }
            else if (rbToday?.IsChecked == true)
            {
                tbDateRange.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
            else if (rbThisWeek?.IsChecked == true)
            {
                var start = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
                var end = start.AddDays(6);
                tbDateRange.Text = $"{start:dd/MM} - {end:dd/MM/yyyy}";
            }
            else if (rbThisMonth?.IsChecked == true)
            {
                tbDateRange.Text = DateTime.Now.ToString("MM/yyyy");
            }
            else if (rbThisYear?.IsChecked == true)
            {
                tbDateRange.Text = DateTime.Now.ToString("yyyy");
            }
            else if (rbCustomRange?.IsChecked == true && dpStartDate?.SelectedDate != null && dpEndDate?.SelectedDate != null)
            {
                tbDateRange.Text = $"{dpStartDate.SelectedDate:dd/MM/yyyy} - {dpEndDate.SelectedDate:dd/MM/yyyy}";
            }
        }
        #endregion

        #region Export Logic
        private (List<ImportBillResponseDto> imports, List<OrderResponseDto> orders) GetFilteredData()
        {
            var imports = _allImports.ToList();
            var orders = _allOrders.ToList();

            // Filter by invoice type
            if (rbImportOnly?.IsChecked == true)
            {
                orders.Clear();
            }
            else if (rbExportOnly?.IsChecked == true)
            {
                imports.Clear();
            }

            // Filter by date range
            if (rbToday?.IsChecked == true)
            {
                var today = DateTime.Now.Date;
                imports = imports.Where(x => x.CreatedDate.Date == today).ToList();
                orders = orders.Where(x => x.CreatedDate.Date == today).ToList();
            }
            else if (rbThisWeek?.IsChecked == true)
            {
                var startOfWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).Date;
                var endOfWeek = startOfWeek.AddDays(7);
                imports = imports.Where(x => x.CreatedDate >= startOfWeek && x.CreatedDate < endOfWeek).ToList();
                orders = orders.Where(x => x.CreatedDate >= startOfWeek && x.CreatedDate < endOfWeek).ToList();
            }
            else if (rbThisMonth?.IsChecked == true)
            {
                var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1);
                imports = imports.Where(x => x.CreatedDate >= startOfMonth && x.CreatedDate < endOfMonth).ToList();
                orders = orders.Where(x => x.CreatedDate >= startOfMonth && x.CreatedDate < endOfMonth).ToList();
            }
            else if (rbThisYear?.IsChecked == true)
            {
                var year = DateTime.Now.Year;
                imports = imports.Where(x => x.CreatedDate.Year == year).ToList();
                orders = orders.Where(x => x.CreatedDate.Year == year).ToList();
            }
            else if (rbCustomRange?.IsChecked == true && dpStartDate?.SelectedDate != null && dpEndDate?.SelectedDate != null)
            {
                var start = dpStartDate.SelectedDate.Value.Date;
                var end = dpEndDate.SelectedDate.Value.Date.AddDays(1);
                imports = imports.Where(x => x.CreatedDate >= start && x.CreatedDate < end).ToList();
                orders = orders.Where(x => x.CreatedDate >= start && x.CreatedDate < end).ToList();
            }

            return (imports, orders);
        }
        #endregion

        #region Button Handlers
        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateExportSettings())
            {
                return;
            }

            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Title = "Lưu file Excel",
                    FileName = $"HoaDon_{DateTime.Now:yyyyMMdd_HHmmss}",
                    DefaultExt = GetFileExtension(),
                    Filter = GetFileFilter()
                };

                if (saveDialog.ShowDialog() == true)
                {
                    ExportToExcel(saveDialog.FileName);

                    var result = MessageBox.Show(
                        $"Xuất Excel thành công!\n\n{saveDialog.FileName}\n\nBạn có muốn mở file?",
                        "Thành công",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information
                    );

                    if (result == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = saveDialog.FileName,
                            UseShellExecute = true
                        });
                    }

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất Excel: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Export Implementation
        private bool ValidateExportSettings()
        {
            if (rbCustomRange?.IsChecked == true)
            {
                if (dpStartDate?.SelectedDate == null)
                {
                    MessageBox.Show("Vui lòng chọn ngày bắt đầu!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    dpStartDate.Focus();
                    return false;
                }

                if (dpEndDate?.SelectedDate == null)
                {
                    MessageBox.Show("Vui lòng chọn ngày kết thúc!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    dpEndDate.Focus();
                    return false;
                }

                if (dpStartDate.SelectedDate > dpEndDate.SelectedDate)
                {
                    MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    dpStartDate.Focus();
                    return false;
                }
            }

            var (imports, orders) = GetFilteredData();
            var totalCount = imports.Count + orders.Count;

            if (totalCount == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!\nVui lòng thay đổi bộ lọc.", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (totalCount > 10000)
            {
                var confirm = MessageBox.Show(
                    $"Dữ liệu có {totalCount:N0} hóa đơn, file có thể rất lớn.\nBạn có chắc muốn tiếp tục?",
                    "Xác nhận",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (confirm != MessageBoxResult.Yes)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Export Methods
        private void ExportToExcel(string filePath)
        {
            var (imports, orders) = GetFilteredData();

            if (rbCsv?.IsChecked == true)
            {
                ExportToCSV(filePath, imports, orders);
            }
            else
            {
                ExportToXLSX(filePath, imports, orders);
            }
        }

        private void ExportToCSV(string filePath, List<ImportBillResponseDto> imports, List<OrderResponseDto> orders)
        {
            using (var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                // Header
                writer.WriteLine("Loại,Mã HĐ,Đối tác,Ngày tạo,Tổng tiền,Người tạo,Ghi chú");

                // Import bills
                foreach (var import in imports.OrderBy(x => x.CreatedDate))
                {
                    writer.WriteLine($"Nhập,{import.Id},{import.PublisherName},{import.CreatedDate:dd/MM/yyyy HH:mm},{import.TotalAmount},{import.CreatedBy},{EscapeCSV(import.Notes)}");
                }

                // Order bills
                foreach (var order in orders.OrderBy(x => x.CreatedDate))
                {
                    writer.WriteLine($"Bán,{order.OrderId},{order.CustomerName ?? "Khách vãng lai"},{order.CreatedDate:dd/MM/yyyy HH:mm},{order.TotalPrice},{order.StaffName},{EscapeCSV(order.Notes)}");
                }

                // Statistics (if enabled)
                if (chkIncludeStatistics?.IsChecked == true)
                {
                    writer.WriteLine();
                    writer.WriteLine("THỐNG KÊ");
                    writer.WriteLine($"Tổng phiếu nhập,{imports.Count}");
                    writer.WriteLine($"Tổng hóa đơn bán,{orders.Count}");
                    writer.WriteLine($"Tổng tiền nhập,{imports.Sum(x => x.TotalAmount)}");
                    writer.WriteLine($"Tổng tiền bán,{orders.Sum(x => x.TotalPrice)}");
                }
            }
        }

        private void ExportToXLSX(string filePath, List<ImportBillResponseDto> imports, List<OrderResponseDto> orders)
        {
            // TODO: Implement using EPPlus or ClosedXML
            // For now, export as CSV with .xlsx extension as placeholder

            MessageBox.Show(
                "Chức năng xuất Excel (.xlsx) đang phát triển.\n" +
                "Hiện tại sẽ xuất dạng CSV.\n\n" +
                "Để sử dụng đầy đủ, cần cài thêm thư viện:\n" +
                "- EPPlus (Free for non-commercial)\n" +
                "- ClosedXML (Open source)",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            // Change extension to CSV
            var csvPath = Path.ChangeExtension(filePath, ".csv");
            ExportToCSV(csvPath, imports, orders);
        }
        #endregion

        #region Helper Methods
        private string EscapeCSV(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }

        private string GetFileExtension()
        {
            if (rbXlsx?.IsChecked == true) return ".xlsx";
            if (rbXls?.IsChecked == true) return ".xls";
            return ".csv";
        }

        private string GetFileFilter()
        {
            if (rbXlsx?.IsChecked == true) return "Excel Files (*.xlsx)|*.xlsx";
            if (rbXls?.IsChecked == true) return "Excel 97-2003 (*.xls)|*.xls";
            return "CSV Files (*.csv)|*.csv";
        }
        #endregion

        #region UI Event Handlers
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            BtnCancel_Click(sender, e);
        }
        #endregion
    }
}