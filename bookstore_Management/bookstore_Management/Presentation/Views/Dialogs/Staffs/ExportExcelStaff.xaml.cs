using bookstore_Management.DTOs.Staff.Responses;
using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace bookstore_Management.Presentation.Views.Dialogs.Staffs
{
    public partial class ExportExcelStaff : Window
    {
        private readonly List<StaffResponseDto> _dataSource;
        private readonly int _totalRecords;

        public ExportExcelStaff(List<StaffResponseDto> staffList)
        {
            InitializeComponent();

            _dataSource = staffList ?? new List<StaffResponseDto>();
            _totalRecords = _dataSource.Count;
            txtTotalRecords.Text = _totalRecords.ToString();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) { DialogResult = false; Close(); }
        private void BtnCancel_Click(object sender, RoutedEventArgs e) { DialogResult = false; Close(); }

        private void chkSelectAll_Checked(object sender, RoutedEventArgs e) => SetAllCheckboxes(true);
        private void chkSelectAll_Unchecked(object sender, RoutedEventArgs e) => SetAllCheckboxes(false);
        private void cbFileFormat_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!HasSelectedColumns())
                {
                    MessageBox.Show("Vui lòng chọn ít nhất 1 cột để xuất!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_dataSource.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để xuất!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedFormat = ((ComboBoxItem)cbFileFormat.SelectedItem).Tag?.ToString() ?? "xlsx";
                string fileName = $"DanhSachNhanVien_{DateTime.Now:yyyyMMdd_HHmmss}";
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
                        MessageBox.Show("Chức năng xuất PDF đang được phát triển!",
                            "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
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

        private void ExportToExcel(string path, string fileName)
        {
            var saveDialog = new SaveFileDialog
            {
                InitialDirectory = path,
                FileName = fileName,
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                DefaultExt = "xlsx"
            };

            if (saveDialog.ShowDialog() != true) return;

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Danh sách nhân viên");

                // Header
                var headers = GetSelectedHeaders();
                for (int c = 0; c < headers.Count; c++)
                {
                    ws.Cell(1, c + 1).Value = headers[c];
                    ws.Cell(1, c + 1).Style.Font.Bold = true;
                    ws.Cell(1, c + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                }

                // Rows
                int row = 2;
                int stt = 1;

                foreach (var s in _dataSource)
                {
                    int col = 1;

                    if (chkSTT.IsChecked == true) ws.Cell(row, col++).Value = stt++;
                    if (chkStaffID.IsChecked == true) ws.Cell(row, col++).Value = s.Id ?? "";
                    if (chkStaffName.IsChecked == true) ws.Cell(row, col++).Value = s.Name ?? "";
                    if (chkCitizenId.IsChecked == true) ws.Cell(row, col++).Value = s.CitizenId ?? "";
                    if (chkPhone.IsChecked == true) ws.Cell(row, col++).Value = s.Phone ?? "";
                    if (chkUserRole.IsChecked == true) ws.Cell(row, col++).Value = s.UserRole.ToString();
                    if (chkTotalOrders.IsChecked == true) ws.Cell(row, col++).Value = s.TotalOrders;
                    if (chkCreatedDate.IsChecked == true) ws.Cell(row, col++).Value = s.CreatedDate.ToString("dd/MM/yyyy HH:mm");

                    row++;
                }

                ws.Columns().AdjustToContents();
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

            if (saveDialog.ShowDialog() != true) return;

            using (var writer = new StreamWriter(saveDialog.FileName, false, Encoding.UTF8))
            {
                var headers = GetSelectedHeaders();
                writer.WriteLine(string.Join(",", headers.Select(h => $"\"{h}\"")));

                int stt = 1;
                foreach (var s in _dataSource)
                {
                    var values = new List<string>();

                    if (chkSTT.IsChecked == true) values.Add(stt++.ToString());
                    if (chkStaffID.IsChecked == true) values.Add($"\"{s.Id ?? ""}\"");
                    if (chkStaffName.IsChecked == true) values.Add($"\"{s.Name ?? ""}\"");
                    if (chkCitizenId.IsChecked == true) values.Add($"\"{s.CitizenId ?? ""}\"");
                    if (chkPhone.IsChecked == true) values.Add($"\"{s.Phone ?? ""}\"");
                    if (chkUserRole.IsChecked == true) values.Add($"\"{s.UserRole}\"");
                    if (chkTotalOrders.IsChecked == true) values.Add(s.TotalOrders.ToString());
                    if (chkCreatedDate.IsChecked == true) values.Add($"\"{s.CreatedDate:dd/MM/yyyy HH:mm}\"");

                    writer.WriteLine(string.Join(",", values));
                }
            }

            MessageBox.Show($"Xuất file thành công!\nĐường dẫn: {saveDialog.FileName}",
                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SetAllCheckboxes(bool isChecked)
        {
            foreach (var child in pnlColumns.Children)
                if (child is CheckBox cb) cb.IsChecked = isChecked;
        }

        private bool HasSelectedColumns()
        {
            foreach (var child in pnlColumns.Children)
                if (child is CheckBox cb && cb.IsChecked == true) return true;
            return false;
        }

        private List<string> GetSelectedHeaders()
        {
            var headers = new List<string>();

            if (chkSTT.IsChecked == true) headers.Add("STT");
            if (chkStaffID.IsChecked == true) headers.Add("Mã nhân viên");
            if (chkStaffName.IsChecked == true) headers.Add("Tên nhân viên");
            if (chkCitizenId.IsChecked == true) headers.Add("CCCD/CMND");
            if (chkPhone.IsChecked == true) headers.Add("Số điện thoại");
            if (chkUserRole.IsChecked == true) headers.Add("Vai trò");
            if (chkTotalOrders.IsChecked == true) headers.Add("Tổng đơn hàng");
            if (chkCreatedDate.IsChecked == true) headers.Add("Ngày tạo");

            return headers;
        }
    }
}
