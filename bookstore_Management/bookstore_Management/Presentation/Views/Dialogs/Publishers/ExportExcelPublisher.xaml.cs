using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClosedXML.Excel;
using Microsoft.Win32;
using bookstore_Management.DTOs.Publisher.Responses;

namespace bookstore_Management.Presentation.Views.Dialogs.Publishers
{
    /// <summary>
    /// Interaction logic for ExportExcelPublisher.xaml
    /// </summary>
    public partial class ExportExcelPublisher : Window
    {
        private List<PublisherResponseDto> _dataSource;
        private int _totalRecords;

        public ExportExcelPublisher()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor với dữ liệu cần xuất
        /// </summary>
        public ExportExcelPublisher(List<PublisherResponseDto> publishers) : this()
        {
            _dataSource = publishers ?? new List<PublisherResponseDto>();
            _totalRecords = _dataSource.Count;
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
                if (!HasSelectedColumns())
                {
                    MessageBox.Show("Vui lòng chọn ít nhất 1 cột để xuất!",
                                  "Thông báo",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                    return;
                }

                if (_dataSource == null || _dataSource.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để xuất!",
                                  "Thông báo",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                    return;
                }

                var selectedFormat = ((ComboBoxItem)cbFileFormat.SelectedItem).Tag.ToString();
                string fileName = $"DanhSachNhaXuatBan_{DateTime.Now:yyyyMMdd_HHmmss}";
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
                var worksheet = workbook.Worksheets.Add("Danh sách nhà xuất bản");

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
                foreach (var publisher in _dataSource)
                {
                    col = 1;
                    if (chkSTT.IsChecked == true)
                        worksheet.Cell(row, col++).Value = stt++;
                    if (chkPublisherID.IsChecked == true)
                        worksheet.Cell(row, col++).Value = publisher.Id;
                    if (chkPublisherName.IsChecked == true)
                        worksheet.Cell(row, col++).Value = publisher.Name;
                    if (chkPhone.IsChecked == true)
                        worksheet.Cell(row, col++).Value = publisher.Phone;
                    if (chkEmail.IsChecked == true)
                        worksheet.Cell(row, col++).Value = publisher.Email ?? "";
                    if (chkCreatedDate.IsChecked == true)
                        worksheet.Cell(row, col++).Value = publisher.CreatedDate.ToString("dd/MM/yyyy");

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
                // Write header
                var headers = GetSelectedHeaders();
                writer.WriteLine(string.Join(",", headers.Select(h => $"\"{h}\"")));

                // Write data
                int stt = 1;
                foreach (var publisher in _dataSource)
                {
                    var values = new List<string>();

                    if (chkSTT.IsChecked == true)
                        values.Add(stt++.ToString());
                    if (chkPublisherID.IsChecked == true)
                        values.Add($"\"{publisher.Id}\"");
                    if (chkPublisherName.IsChecked == true)
                        values.Add($"\"{publisher.Name}\"");
                    if (chkPhone.IsChecked == true)
                        values.Add($"\"{publisher.Phone}\"");
                    if (chkEmail.IsChecked == true)
                        values.Add($"\"{publisher.Email ?? ""}\"");
                    if (chkCreatedDate.IsChecked == true)
                        values.Add($"\"{publisher.CreatedDate:dd/MM/yyyy}\"");

                    writer.WriteLine(string.Join(",", values));
                }
            }

            MessageBox.Show($"Xuất file thành công!\nĐường dẫn: {saveDialog.FileName}",
                          "Thành công",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }

        private void ExportToPdf(string path, string fileName)
        {
            MessageBox.Show("Chức năng xuất PDF đang được phát triển!",
                          "Thông báo",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
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

        private List<string> GetSelectedHeaders()
        {
            var headers = new List<string>();

            if (chkSTT.IsChecked == true)
                headers.Add("STT");
            if (chkPublisherID.IsChecked == true)
                headers.Add("Mã NXB");
            if (chkPublisherName.IsChecked == true)
                headers.Add("Tên nhà xuất bản");
            if (chkPhone.IsChecked == true)
                headers.Add("Số điện thoại");
            if (chkEmail.IsChecked == true)
                headers.Add("Email");
            if (chkCreatedDate.IsChecked == true)
                headers.Add("Ngày tạo");

            return headers;
        }

        #endregion
    }
}