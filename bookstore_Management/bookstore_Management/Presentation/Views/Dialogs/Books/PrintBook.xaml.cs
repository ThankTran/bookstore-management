using bookstore_Management.Models;   
using bookstore_Management.Presentation.ViewModels; 
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Printing;          
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Xps;        
using System.Windows.Xps.Packaging;
using System.Text.RegularExpressions;

namespace bookstore_Management.Presentation.Views.Dialogs.Books 
{
    public partial class PrintBook : Window
    {
        private PrintViewModel _viewModel;
        private string _documentTitle = "Danh sách sách";

        // Constructor nhận dữ liệu từ bên ngoài vào
        public PrintBook(IEnumerable<Book> dataToPrint)
        {
            InitializeComponent();

            var columns = new List<PrintColumnDef>
            {
                // ... code tạo cột của bạn giữ nguyên ...
                new PrintColumnDef { Header = "Tên Sách", PropertyPath = "Name" }, 
                new PrintColumnDef { Header = "Tác Giả", PropertyPath = "Author" }  
            };

            _viewModel = new PrintViewModel(dataToPrint, columns, _documentTitle);

            // 3. Gán DataContext để giao diện nhận dữ liệu
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

        // Hàm helper quan trọng: Tạo Document từ ViewModel
        private FlowDocument CreatePrintDocument()
        {
            // Lấy khổ giấy từ ComboBox trên giao diện
            if (cbPaperSize.SelectedItem is ComboBoxItem item)
            {
                _viewModel.PaperSize = item.Content.ToString();
            }

            // Lấy hướng giấy (Dọc/Ngang)
            _viewModel.IsPortrait = rbPortrait.IsChecked == true;

            // Gọi hàm tạo document từ ViewModel
            return _viewModel.CreateDocument();
        }

        private void BtnPreview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var doc = CreatePrintDocument();

                // Hiển thị vùng xem trước
                PreviewContainer.Visibility = Visibility.Visible;
                DocPreview.Document = doc;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xem trước: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lấy thông tin từ UI
                if (!int.TryParse(txtCopies.Text, out int copies) || copies < 1) copies = 1;
                bool savePDF = chkSavePDF.IsChecked ?? false;

                // Xác nhận in
                var result = MessageBox.Show(
                    $"Bạn có chắc muốn in {copies} bản {_documentTitle}?",
                    "Xác nhận in",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.No) return;

                // 3. Khởi tạo PrintDialog
                PrintDialog printDialog = new PrintDialog();

                if (printDialog.ShowDialog() == true)
                {
                    // Tạo document mới nhất theo khổ giấy máy in vừa chọn
                    var document = CreatePrintDocument();

                    document.PageWidth = printDialog.PrintableAreaWidth;

                    if (printDialog.PrintTicket != null)
                    {
                        printDialog.PrintTicket.CopyCount = copies;
                    }

                    // Thực hiện lệnh in
                    printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, _documentTitle);

                    // 4. Xử lý lưu File (Logic phụ)
                    if (savePDF)
                    {
                        SaveAsXps(document);
                    }

                    MessageBox.Show("Đã gửi lệnh in thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi in: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveAsXps(FlowDocument doc)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "DanhSachSach_" + DateTime.Now.ToString("ddMMyyyy_HHmm");
                dlg.DefaultExt = ".xps";
                dlg.Filter = "XPS Documents (.xps)|*.xps";

                if (dlg.ShowDialog() == true)
                {
                    // Tạo file XPS
                    using (Package package = Package.Open(dlg.FileName, FileMode.Create))
                    {
                        using (XpsDocument xpsDoc = new XpsDocument(package, CompressionOption.Maximum))
                        {
                            XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
                            // Ghi nội dung document vào file
                            xpsWriter.Write(((IDocumentPaginatorSource)doc).DocumentPaginator);
                        }
                    }
                    MessageBox.Show($"Đã lưu file tại:\n{dlg.FileName}", "Lưu thành công");
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