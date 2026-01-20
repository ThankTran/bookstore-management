using bookstore_Management.DTOs.ImportBill.Responses;
using bookstore_Management.DTOs.Order.Responses;
using bookstore_Management.Presentation.Views.Orders;
using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using bookstore_Management.Presentation.ViewModels;

namespace bookstore_Management.Presentation.Views.Dialogs.Invoices
{
    public partial class PrintInvoice : Window
    {
        private readonly InvoiceType _invoiceType;
        private readonly string _invoiceId;
        private readonly object _invoiceData; // ImportBillResponseDto or OrderResponseDto

        public PrintInvoice(string invoiceId, InvoiceType invoiceType, object invoiceData)
        {
            InitializeComponent();

            _invoiceId = invoiceId;
            _invoiceType = invoiceType;
            _invoiceData = invoiceData;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPrinters();
            LoadInvoiceData();
            AttachCheckboxEvents();
        }

        #region Printer Setup
        private void LoadPrinters()
        {
            try
            {
                foreach (string printer in PrinterSettings.InstalledPrinters)
                {
                    cbPrinter.Items.Add(printer);
                }

                // Set default printer
                var defaultPrinter = new PrinterSettings().PrinterName;
                cbPrinter.SelectedItem = defaultPrinter;

                if (cbPrinter.SelectedIndex < 0 && cbPrinter.Items.Count > 0)
                {
                    cbPrinter.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách máy in: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        #endregion

        #region Load Invoice Data
        private void LoadInvoiceData()
        {
            if (_invoiceType == InvoiceType.Import && _invoiceData is ImportBillResponseDto import)
            {
                tbPreviewTitle.Text = "PHIẾU NHẬP HÀNG";
                tbInvoiceInfo.Text = $"Phiếu nhập: {import.Id}";
                tbPreviewId.Text = import.Id;
                tbPreviewDate.Text = import.CreatedDate.ToString("dd/MM/yyyy HH:mm");
                tbPreviewCustomer.Text = $"NXB: {import.PublisherName}";
            }
            else if (_invoiceType == InvoiceType.Export && _invoiceData is OrderResponseDto order)
            {
                tbPreviewTitle.Text = "HÓA ĐƠN BÁN HÀNG";
                tbInvoiceInfo.Text = $"Hóa đơn: {order.OrderId}";
                tbPreviewId.Text = order.OrderId;
                tbPreviewDate.Text = order.CreatedDate.ToString("dd/MM/yyyy HH:mm");
                tbPreviewCustomer.Text = order.CustomerName ?? "Khách vãng lai";
            }
        }
        #endregion

        #region Checkbox Preview Handling 
        private void AttachCheckboxEvents()
        {
            chkShowLogo.Checked += UpdatePreview;
            chkShowLogo.Unchecked += UpdatePreview;

            chkShowCompanyInfo.Checked += UpdatePreview;
            chkShowCompanyInfo.Unchecked += UpdatePreview;

            chkShowBarcode.Checked += UpdatePreview;
            chkShowBarcode.Unchecked += UpdatePreview;

            chkShowNotes.Checked += UpdatePreview;
            chkShowNotes.Unchecked += UpdatePreview;

            chkShowSignature.Checked += UpdatePreview;
            chkShowSignature.Unchecked += UpdatePreview;
        }



        private void UpdatePreview(object sender, RoutedEventArgs e)
        {
            previewLogo.Visibility = chkShowLogo.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            previewCompanyInfo.Visibility = chkShowCompanyInfo.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            previewBarcode.Visibility = chkShowBarcode.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            previewNotes.Visibility = chkShowNotes.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            previewSignature.Visibility = chkShowSignature.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        #region Copy control
        private void BtnIncreaseCopies_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(tbCopies.Text, out int current))
            {
                if (current < 99)
                {
                    tbCopies.Text = (current + 1).ToString();
                }
            }
        }

        private void BtnDecreaseCopies_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(tbCopies.Text, out int current))
            {
                if (current > 1)
                {
                    tbCopies.Text = (current - 1).ToString();
                }
            }
        }

        #endregion

        #region Button
        private void BtnPreview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Generate full preview window
                var previewWindow = new Window
                {
                    Title = "Xem trước hóa đơn",
                    Width = 800,
                    Height = 1000,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Content = GenerateFullPreview()
                };

                previewWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xem trước: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSavePDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    FileName = $"{_invoiceId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                    Title = "Lưu hóa đơn dạng PDF"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    SaveAsPDF(saveDialog.FileName);

                    var result = MessageBox.Show(
                        $"Đã lưu file PDF thành công!\n\n{saveDialog.FileName}\n\nBạn có muốn mở file?",
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu PDF: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidatePrintSettings())
            {
                return;
            }

            var confirm = MessageBox.Show(
                $"In {tbCopies.Text} bản hóa đơn {_invoiceId}?\n\n" +
                $"Máy in: {cbPrinter.SelectedItem}\n" +
                $"Khổ giấy: {((ComboBoxItem)cbPaperSize.SelectedItem).Content}",
                "Xác nhận in",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                Print();

                MessageBox.Show(
                    $"Đã gửi lệnh in {tbCopies.Text} bản hóa đơn thành công!",
                    "Thành công",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi in hóa đơn: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Validation
        private bool ValidatePrintSettings()
        {
            if (cbPrinter.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn máy in!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cbPrinter.Focus();
                return false;
            }

            if (!int.TryParse(tbCopies.Text, out int copies) || copies < 1 || copies > 99)
            {
                MessageBox.Show("Số bản in phải từ 1 đến 99!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbCopies.Focus();
                return false;
            }

            return true;
        }

        #endregion

        #region Print & Export
        private void Print()
        {
            var printDoc = new PrintDocument();
            printDoc.PrinterSettings.PrinterName = cbPrinter.SelectedItem.ToString();
            printDoc.PrinterSettings.Copies = short.Parse(tbCopies.Text);

            // Set paper size
            switch (cbPaperSize.SelectedIndex)
            {
                case 0: // A4
                    printDoc.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);
                    break;
                case 1: // A5
                    printDoc.DefaultPageSettings.PaperSize = new PaperSize("A5", 583, 827);
                    break;
                case 2: // K80
                    printDoc.DefaultPageSettings.PaperSize = new PaperSize("K80", 315, 1000);
                    break;
                case 3: // Letter
                    printDoc.DefaultPageSettings.PaperSize = new PaperSize("Letter", 850, 1100);
                    break;
            }

            // Set orientation
            printDoc.DefaultPageSettings.Landscape = cbOrientation.SelectedIndex == 1;

            printDoc.PrintPage += (s, ev) =>
            {
                var g = ev.Graphics;
                var font = new System.Drawing.Font("Arial", 12);
                var bold = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
                var brush = System.Drawing.Brushes.Black;

                float y = 20;

                if (_invoiceType == InvoiceType.Import && _invoiceData is ImportBillResponseDto import)
                {
                    g.DrawString("PHIẾU NHẬP HÀNG", bold, brush, 20, y); y += 30;
                    g.DrawString($"Mã phiếu: {import.Id}", font, brush, 20, y); y += 25;
                    g.DrawString($"Ngày: {import.CreatedDate:dd/MM/yyyy HH:mm}", font, brush, 20, y); y += 25;
                    g.DrawString($"NXB: {import.PublisherName}", font, brush, 20, y); y += 25;
                    g.DrawString($"Tổng tiền: {import.TotalAmount:N0} ₫", font, brush, 20, y);
                }
                else if (_invoiceType == InvoiceType.Export && _invoiceData is OrderResponseDto order)
                {
                    g.DrawString("HÓA ĐƠN BÁN HÀNG", bold, brush, 20, y); y += 30;
                    g.DrawString($"Mã HĐ: {order.OrderId}", font, brush, 20, y); y += 25;
                    g.DrawString($"Ngày: {order.CreatedDate:dd/MM/yyyy HH:mm}", font, brush, 20, y); y += 25;
                    g.DrawString($"Khách hàng: {order.CustomerName ?? "Khách vãng lai"}", font, brush, 20, y); y += 25;
                    g.DrawString($"Tổng tiền: {order.TotalPrice:N0} ₫", font, brush, 20, y);
                }
            };

            printDoc.Print();
        }


        private void SaveAsPDF(string filePath)
        {
            // TODO: Implement PDF generation
            // You can use libraries like iTextSharp, PdfSharp, or QuestPDF

            // For now, create a simple text file as placeholder
            var content = GeneratePDFContent();
            File.WriteAllText(filePath.Replace(".pdf", ".txt"), content);

            MessageBox.Show(
                "Chức năng xuất PDF đang trong quá trình phát triển.\n" +
                "Hiện tại đã lưu file text thay thế.",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private string GeneratePDFContent()
        {
            var content = $"HÓA ĐƠN: {_invoiceId}\n";
            content += $"Ngày: {DateTime.Now:dd/MM/yyyy HH:mm}\n";
            content += new string('-', 50) + "\n";

            if (_invoiceType == InvoiceType.Import && _invoiceData is ImportBillResponseDto import)
            {
                content += $"PHIẾU NHẬP HÀNG\n";
                content += $"Nhà xuất bản: {import.PublisherName}\n";
                content += $"Tổng tiền: {import.TotalAmount:N0} ₫\n";
            }
            else if (_invoiceType == InvoiceType.Export && _invoiceData is OrderResponseDto order)
            {
                content += $"HÓA ĐƠN BÁN HÀNG\n";
                content += $"Khách hàng: {order.CustomerName ?? "Khách vãng lai"}\n";
                content += $"Tổng tiền: {order.TotalPrice:N0} ₫\n";
            }

            return content;
        }

        #endregion

        #region UI Helpers
        private ScrollViewer GenerateFullPreview()
        {
            var viewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Background = new SolidColorBrush(Colors.LightGray),
                Padding = new Thickness(20)
            };

            var border = new Border
            {
                Background = new SolidColorBrush(Colors.White),
                Width = 700,
                Padding = new Thickness(40),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var stack = new StackPanel();

            if (chkShowLogo.IsChecked == true)
            {
                stack.Children.Add(new TextBlock
                {
                    Text = "📚 NHÀ SÁCH XYZ",
                    FontSize = 24,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 20)
                });
            }

            stack.Children.Add(new TextBlock
            {
                Text = _invoiceType == InvoiceType.Import ? "PHIẾU NHẬP HÀNG" : "HÓA ĐƠN BÁN HÀNG",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            });

            stack.Children.Add(new TextBlock
            {
                Text = $"Mã: {_invoiceId}",
                FontSize = 14,
                Margin = new Thickness(0, 5, 0, 5)
            });

            stack.Children.Add(new TextBlock
            {
                Text = $"Ngày: {DateTime.Now:dd/MM/yyyy HH:mm}",
                FontSize = 14,
                Margin = new Thickness(0, 5, 0, 5)
            });

            border.Child = stack;
            viewer.Content = border;

            return viewer;
        }

        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void CbPrinter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update preview or settings based on selected printer
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        #endregion
    }

}