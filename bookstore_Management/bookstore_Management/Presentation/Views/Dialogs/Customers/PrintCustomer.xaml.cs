using bookstore_Management.Models;
using bookstore_Management.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace bookstore_Management.Presentation.Views.Dialogs.Customers
{
    public partial class PrintCustomer : Window
    {
        private readonly PrintViewModel _viewModel;
        private readonly string _documentTitle = "Danh sách khách hàng";

        public PrintCustomer(IEnumerable<Customer> dataToPrint)
        {
            InitializeComponent();

            // 1. ĐỊNH NGHĨA CỘT IN (GIỐNG STAFF / BOOK)
            var columns = new List<PrintColumnDef>
            {
                new PrintColumnDef { Header = "Mã KH", PropertyPath = "CustomerCode" },
                new PrintColumnDef { Header = "Tên khách hàng", PropertyPath = "FullName" },
                new PrintColumnDef { Header = "SĐT", PropertyPath = "PhoneNumber" },
                new PrintColumnDef { Header = "Email", PropertyPath = "Email" },
                new PrintColumnDef { Header = "Ngày tạo", PropertyPath = "CreatedDate" }
            };

            // 2. KHỞI TẠO VIEWMODEL
            _viewModel = new PrintViewModel(dataToPrint, columns, _documentTitle);

            // 3. GÁN DATACONTEXT
            DataContext = _viewModel;

            // UI
            txtTitle.Text = $"In {_documentTitle}";
            txtRecordCount.Text = $"• Sẽ in {dataToPrint?.Count() ?? 0} khách hàng";
        }

        #region UI EVENTS

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnIncrease_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtCopies.Text, out int copies) && copies < 99)
                txtCopies.Text = (copies + 1).ToString();
        }

        private void BtnDecrease_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtCopies.Text, out int copies) && copies > 1)
                txtCopies.Text = (copies - 1).ToString();
        }

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        #endregion

        #region PRINT CORE

        private FlowDocument CreatePrintDocument()
        {
            if (cbPaperSize.SelectedItem is ComboBoxItem item)
                _viewModel.PaperSize = item.Content.ToString();

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
                MessageBox.Show($"Lỗi xem trước: {ex.Message}");
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int copies = int.TryParse(txtCopies.Text, out int c) ? c : 1;
                bool saveXps = chkSavePDF.IsChecked ?? false;

                var confirm = MessageBox.Show(
                    $"Bạn có chắc muốn in {copies} bản {_documentTitle}?",
                    "Xác nhận in",
                    MessageBoxButton.YesNo);

                if (confirm == MessageBoxResult.No) return;

                var printDialog = new PrintDialog();

                if (printDialog.ShowDialog() == true)
                {
                    var document = CreatePrintDocument();
                    document.PageWidth = printDialog.PrintableAreaWidth;

                    if (printDialog.PrintTicket != null)
                        printDialog.PrintTicket.CopyCount = copies;

                    printDialog.PrintDocument(
                        ((IDocumentPaginatorSource)document).DocumentPaginator,
                        _documentTitle);

                    if (saveXps)
                        SaveAsXps(document);

                    MessageBox.Show("In thành công!");
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi in: {ex.Message}");
            }
        }

        private void SaveAsXps(FlowDocument doc)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "DanhSachKhachHang_" + DateTime.Now.ToString("ddMMyyyy_HHmm"),
                DefaultExt = ".xps",
                Filter = "XPS Documents (*.xps)|*.xps"
            };

            if (dlg.ShowDialog() == true)
            {
                using (var package = System.IO.Packaging.Package.Open(dlg.FileName, System.IO.FileMode.Create))
                using (var xpsDoc = new System.Windows.Xps.Packaging.XpsDocument(package,
                    System.IO.Packaging.CompressionOption.Maximum))
                {
                    var writer = System.Windows.Xps.Packaging.XpsDocument.CreateXpsDocumentWriter(xpsDoc);
                    writer.Write(((IDocumentPaginatorSource)doc).DocumentPaginator);
                }

                MessageBox.Show("Lưu file thành công!");
            }
        }

        #endregion
    }
}
