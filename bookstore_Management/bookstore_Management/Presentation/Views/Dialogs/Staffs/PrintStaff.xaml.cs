using bookstore_Management.Models;
using bookstore_Management.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace bookstore_Management.Presentation.Views.Dialogs.Staffs
{    public partial class PrintStaff : Window
    {
        private PrintViewModel _viewModel;
        private string _documentTitle = "Danh sách nhân viên";

        // Constructor nhận dữ liệu từ bên ngoài vào
        public PrintStaff(IEnumerable<Staff> dataToPrint)
        {
            InitializeComponent();

            // 1. Khai báo các cột in (GIỐNG BOOK)
            var columns = new List<PrintColumnDef>
            {
                new PrintColumnDef { Header = "Mã NV", PropertyPath = "StaffCode" },
                new PrintColumnDef { Header = "Tên Nhân Viên", PropertyPath = "FullName" },
                new PrintColumnDef { Header = "Chức Vụ", PropertyPath = "Position" },
                new PrintColumnDef { Header = "SĐT", PropertyPath = "PhoneNumber" },
                new PrintColumnDef { Header = "Email", PropertyPath = "Email" },
                new PrintColumnDef { Header = "Ngày Vào Làm", PropertyPath = "JoinDate" }
            };

            // 2. KHỞI TẠO VIEWMODEL
            _viewModel = new PrintViewModel(dataToPrint, columns, _documentTitle);

            // 3. Gán DataContext
            this.DataContext = _viewModel;
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // ================== CORE ==================

        private FlowDocument CreatePrintDocument()
        {
            // Lấy khổ giấy
            if (cbPaperSize.SelectedItem is ComboBoxItem item)
            {
                _viewModel.PaperSize = item.Content.ToString();
            }

            // Lấy hướng giấy
            _viewModel.IsPortrait = rbPortrait.IsChecked == true;

            return _viewModel.CreateDocument();
        }

        private void BtnPreview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var doc = CreatePrintDocument();
                PreviewContainer.Visibility = Visibility.Visible;
                DocPreview.Document = doc;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xem trước: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtCopies.Text, out int copies) || copies < 1) copies = 1;
                bool savePDF = chkSavePDF.IsChecked ?? false;

                var confirm = MessageBox.Show(
                    $"Bạn có chắc muốn in {copies} bản {_documentTitle}?",
                    "Xác nhận in",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirm == MessageBoxResult.No) return;

                PrintDialog printDialog = new PrintDialog();

                if (printDialog.ShowDialog() == true)
                {
                    var document = CreatePrintDocument();

                    document.PageWidth = printDialog.PrintableAreaWidth;

                    if (printDialog.PrintTicket != null)
                        printDialog.PrintTicket.CopyCount = copies;

                    printDialog.PrintDocument(
                        ((IDocumentPaginatorSource)document).DocumentPaginator,
                        _documentTitle);

                    if (savePDF)
                    {
                        SaveAsXps(document);
                    }

                    MessageBox.Show("In thành công!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi in: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ================== UTIL ==================

        private void SaveAsXps(FlowDocument doc)
        {
            try
            {
                var dlg = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = "DanhSachNhanVien_" + DateTime.Now.ToString("ddMMyyyy_HHmm"),
                    DefaultExt = ".xps",
                    Filter = "XPS Documents (.xps)|*.xps"
                };

                if (dlg.ShowDialog() == true)
                {
                    using (var package = System.IO.Packaging.Package.Open(dlg.FileName, System.IO.FileMode.Create))
                    using (var xpsDoc = new System.Windows.Xps.Packaging.XpsDocument(package, System.IO.Packaging.CompressionOption.Maximum))
                    {
                        var writer = System.Windows.Xps.Packaging.XpsDocument.CreateXpsDocumentWriter(xpsDoc);
                        writer.Write(((IDocumentPaginatorSource)doc).DocumentPaginator);
                    }

                    MessageBox.Show("Lưu file thành công!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể lưu file: " + ex.Message);
            }
        }

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
