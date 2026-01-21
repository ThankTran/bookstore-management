using bookstore_Management.Models;
using bookstore_Management.Services.Implementations;
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

namespace bookstore_Management.Presentation.Views.Dialogs.Customers
{
    /// <summary>
    /// Interaction logic for ExportExcelCustomer.xaml
    /// </summary>

    public partial class ExportExcelCustomer : Window
    {
        private readonly ICustomerService _customerService;
        private int _totalRecords;

        /// <summary>
        /// Constructor - chỉ cần service, không cần truyền data
        /// </summary>
        public ExportExcelCustomer(List<Customer> customers)
        {
            InitializeComponent();

            // Lấy tổng số từ service
            var result = customers;
            _totalRecords = result.Count();
            txtTotalRecords.Text = _totalRecords.ToString();
        }

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
                // Kiểm tra có chọn ít nhất 1 cột
                if (!HasSelectedColumns())
                {
                    MessageBox.Show("Vui lòng chọn ít nhất 1 cột để xuất!",
                                  "Thông báo",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                    return;
                }

                //  LẤY DỮ LIỆU TỪ SERVICE
                var result = _customerService.GetCustomerList();
                if (!result.IsSuccess || result.Data == null || !result.Data.Any())
                {
                    MessageBox.Show(result.ErrorMessage ?? "Không có dữ liệu để xuất!",
                                  "Thông báo",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                    return;
                }

                // Lấy format được chọn
                var selectedFormat = ((ComboBoxItem)cbFileFormat.SelectedItem).Tag.ToString();

                // Tạo tên file
                string fileName = $"DanhSachKhachHang_{DateTime.Now:yyyyMMdd_HHmmss}";
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
                              "Lỗi",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        #endregion

        #region Export Methods

        /// <summary>
        /// Xuất ra file Excel
        /// </summary>
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

            // ✅ LẤY DỮ LIỆU TỪ SERVICE
            var result = _customerService.GetCustomerList();
            if (!result.IsSuccess || result.Data == null)
            {
                MessageBox.Show(result.ErrorMessage ?? "Không lấy được danh sách khách hàng để xuất!",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var data = result.Data.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Danh sách khách hàng");

                // Tạo header
                int col = 1;
                var headers = GetSelectedHeaders();
                foreach (var header in headers)
                {
                    worksheet.Cell(1, col).Value = header;
                    worksheet.Cell(1, col).Style.Font.Bold = true;
                    worksheet.Cell(1, col).Style.Fill.BackgroundColor = XLColor.LightBlue;
                    col++;
                }

                // Thêm dữ liệu
                int row = 2;
                int stt = 1;
                foreach (var customer in data)
                {
                    col = 1;

                    if (chkSTT.IsChecked == true)
                        worksheet.Cell(row, col++).Value = stt++;

                    if (chkCustomerID.IsChecked == true)
                        worksheet.Cell(row, col++).Value = customer.CustomerId ?? "";

                    if (chkCustomerName.IsChecked == true)
                        worksheet.Cell(row, col++).Value = customer.Name ?? "";

                    if (chkPhone.IsChecked == true)
                        worksheet.Cell(row, col++).Value = customer.Phone ?? "";

                    if (chkEmail.IsChecked == true)
                        worksheet.Cell(row, col++).Value = customer.Email ?? "";

                    if (chkAddress.IsChecked == true)
                        worksheet.Cell(row, col++).Value = customer.Address ?? "";

                    if (chkRank.IsChecked == true)
                        worksheet.Cell(row, col++).Value = customer.MemberLevel.ToString();

                    if (chkPoint.IsChecked == true)
                        worksheet.Cell(row, col++).Value = customer.LoyaltyPoints;

                    row++;
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(saveDialog.FileName);

                MessageBox.Show($"Xuất file thành công!\nĐường dẫn: {saveDialog.FileName}",
                              "Thành công",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Xuất ra file CSV
        /// </summary>
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

            // ✅ LẤY DỮ LIỆU TỪ SERVICE
            var result = _customerService.GetCustomerList();
            if (!result.IsSuccess || result.Data == null)
            {
                MessageBox.Show(result.ErrorMessage ?? "Không lấy được danh sách khách hàng để xuất!",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var data = result.Data.ToList();

            using (var writer = new StreamWriter(saveDialog.FileName, false, System.Text.Encoding.UTF8))
            {
                var headers = GetSelectedHeaders();
                writer.WriteLine(string.Join(",", headers.Select(h => $"\"{h}\"")));

                int stt = 1;
                foreach (var customer in data)
                {
                    var values = new List<string>();

                    if (chkSTT.IsChecked == true) values.Add(stt++.ToString());
                    if (chkCustomerID.IsChecked == true) values.Add($"\"{customer.CustomerId ?? ""}\"");
                    if (chkCustomerName.IsChecked == true) values.Add($"\"{customer.Name ?? ""}\"");
                    if (chkPhone.IsChecked == true) values.Add($"\"{customer.Phone ?? ""}\"");
                    if (chkEmail.IsChecked == true) values.Add($"\"{customer.Email ?? ""}\"");
                    if (chkAddress.IsChecked == true) values.Add($"\"{customer.Address ?? ""}\"");
                    if (chkRank.IsChecked == true) values.Add($"\"{customer.MemberLevel}\"");
                    if (chkPoint.IsChecked == true) values.Add(customer.LoyaltyPoints.ToString());

                    writer.WriteLine(string.Join(",", values));
                }
            }

            MessageBox.Show($"Xuất file thành công!\nĐường dẫn: {saveDialog.FileName}",
                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Xuất ra file PDF
        /// </summary>
        private void ExportToPdf(string path, string fileName)
        {
            // Implement PDF export using iTextSharp or similar library
            MessageBox.Show("Chức năng xuất PDF đang được phát triển!",
                          "Thông báo",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Set tất cả checkbox
        /// </summary>
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

        /// <summary>
        /// Kiểm tra có chọn cột nào không
        /// </summary>
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

        /// <summary>
        /// Lấy danh sách header đã chọn
        /// </summary>
        private List<string> GetSelectedHeaders()
        {
            var headers = new List<string>();

            if (chkSTT.IsChecked == true) headers.Add("STT");
            if (chkCustomerID.IsChecked == true) headers.Add("Mã KH");
            if (chkCustomerName.IsChecked == true) headers.Add("Tên KH");
            if (chkPhone.IsChecked == true) headers.Add("SĐT");
            if (chkEmail.IsChecked == true) headers.Add("Email");
            if (chkAddress.IsChecked == true) headers.Add("Địa chỉ");
            if (chkRank.IsChecked == true) headers.Add("Hạng");
            if (chkPoint.IsChecked == true) headers.Add("Điểm");

            return headers;
        }

        #endregion
    }
}
