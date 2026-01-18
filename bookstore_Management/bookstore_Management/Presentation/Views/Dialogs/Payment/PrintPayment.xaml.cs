using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using bookstore_Management.DTOs.Order.Responses;
using bookstore_Management.Services.Interfaces;

namespace bookstore_Management.Presentation.Views.Dialogs.Payment
{
    public partial class PrintPayment : Window
    {
        private readonly IOrderService _orderService;
        private readonly string _orderId;
        private OrderResponseDto _orderData;

        public PrintPayment(IOrderService orderService, string orderId)
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
                txtDate.Text = _orderData.CreatedDate.ToString("dd/MM/yyyy HH:mm");
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

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnIncrease_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtCopies.Text, out int copies))
            {
                if (copies < 99)
                {
                    txtCopies.Text = (copies + 1).ToString();
                }
            }
        }

        private void BtnDecrease_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtCopies.Text, out int copies))
            {
                if (copies > 1)
                {
                    txtCopies.Text = (copies - 1).ToString();
                }
            }
        }

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BtnPreview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var doc = CreateInvoiceDocument();
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
                int copies = int.Parse(txtCopies.Text);
                bool savePDF = chkSavePDF.IsChecked ?? false;

                var result = MessageBox.Show(
                    $"Bạn có chắc muốn in {copies} bản hóa đơn {_orderId}?",
                    "Xác nhận in",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                    return;

                var printDialog = new PrintDialog();

                if (printDialog.ShowDialog() == true)
                {
                    var document = CreateInvoiceDocument();

                    // In bản khách hàng
                    if (chkPrintCustomerCopy.IsChecked == true)
                    {
                        for (int i = 0; i < copies; i++)
                        {
                            printDialog.PrintDocument(
                                ((IDocumentPaginatorSource)document).DocumentPaginator,
                                $"HoaDon_{_orderId}_KhachHang");
                        }
                    }

                    // In bản lưu trữ
                    if (chkPrintStoreCopy.IsChecked == true)
                    {
                        printDialog.PrintDocument(
                            ((IDocumentPaginatorSource)document).DocumentPaginator,
                            $"HoaDon_{_orderId}_LuuTru");
                    }

                    if (savePDF)
                    {
                        SaveAsPDF(document);
                    }

                    MessageBox.Show("In hóa đơn thành công!",
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi in: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Helper Methods

        private FlowDocument CreateInvoiceDocument()
        {
            var doc = new FlowDocument
            {
                PagePadding = new Thickness(40),
                FontFamily = new FontFamily("Arial"),
                FontSize = 12
            };

            // Header - Store Info
            var headerPara = new Paragraph
            {
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            headerPara.Inlines.Add(new Run("BOOKSTORE MANAGEMENT\n")
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold
            });
            headerPara.Inlines.Add(new Run("Địa chỉ: 123 Đường ABC, Quận XYZ\n"));
            headerPara.Inlines.Add(new Run("Điện thoại: 0123-456-789\n"));
            doc.Blocks.Add(headerPara);

            // Title
            var titlePara = new Paragraph(new Run("HÓA ĐƠN BÁN HÀNG"))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 10, 0, 20)
            };
            doc.Blocks.Add(titlePara);

            // Order Info
            var infoPara = new Paragraph
            {
                Margin = new Thickness(0, 0, 0, 15)
            };
            infoPara.Inlines.Add(new Run($"Mã đơn hàng: {_orderData.OrderId}\n"));
            infoPara.Inlines.Add(new Run($"Ngày: {_orderData.CreatedDate:dd/MM/yyyy HH:mm}\n"));
            infoPara.Inlines.Add(new Run($"Nhân viên: {_orderData.StaffName ?? "N/A"}\n"));
            infoPara.Inlines.Add(new Run($"Khách hàng: {_orderData.CustomerName ?? "Khách vãng lai"}\n"));
            infoPara.Inlines.Add(new Run($"Phương thức: {_orderData.PaymentMethod}\n"));
            doc.Blocks.Add(infoPara);

            // Product Table (if show details)
            if (chkShowDetails.IsChecked == true && _orderData.OrderDetails != null)
            {
                var table = new Table
                {
                    CellSpacing = 0,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1)
                };

                // Columns
                table.Columns.Add(new TableColumn { Width = new GridLength(40) }); // STT
                table.Columns.Add(new TableColumn { Width = new GridLength(200) }); // Tên
                table.Columns.Add(new TableColumn { Width = new GridLength(60) }); // SL
                table.Columns.Add(new TableColumn { Width = new GridLength(80) }); // Đơn giá
                table.Columns.Add(new TableColumn { Width = new GridLength(100) }); // Thành tiền

                var rowGroup = new TableRowGroup();
                table.RowGroups.Add(rowGroup);

                // Header
                var headerRow = new TableRow { Background = Brushes.LightGray };
                AddTableCell(headerRow, "STT", true);
                AddTableCell(headerRow, "Tên sản phẩm", true);
                AddTableCell(headerRow, "SL", true);
                AddTableCell(headerRow, "Đơn giá", true);
                AddTableCell(headerRow, "Thành tiền", true);
                rowGroup.Rows.Add(headerRow);

                // Data rows
                int stt = 1;
                foreach (var detail in _orderData.OrderDetails)
                {
                    var dataRow = new TableRow();
                    AddTableCell(dataRow, stt++.ToString());
                    AddTableCell(dataRow, detail.BookName);
                    AddTableCell(dataRow, detail.Quantity.ToString());
                    AddTableCell(dataRow, $"{detail.SalePrice:N0}");
                    AddTableCell(dataRow, $"{detail.Subtotal:N0}");
                    rowGroup.Rows.Add(dataRow);
                }

                doc.Blocks.Add(table);
            }

            // Summary
            var summaryPara = new Paragraph
            {
                Margin = new Thickness(0, 20, 0, 0),
                TextAlignment = TextAlignment.Right
            };
            summaryPara.Inlines.Add(new Run($"Tạm tính: {_orderData.TotalPrice / (1 - _orderData.Discount):N0} ₫\n"));
            summaryPara.Inlines.Add(new Run($"Giảm giá: {_orderData.Discount * 100:N0}%\n"));
            summaryPara.Inlines.Add(new Run($"Tổng cộng: {_orderData.TotalPrice:N0} ₫\n")
            {
                FontSize = 14,
                FontWeight = FontWeights.Bold
            });
            doc.Blocks.Add(summaryPara);

            // Footer
            var footerPara = new Paragraph
            {
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 30, 0, 0),
                FontSize = 10
            };
            footerPara.Inlines.Add(new Run("Cảm ơn quý khách đã mua hàng!\n"));
            footerPara.Inlines.Add(new Run("Hẹn gặp lại!"));
            doc.Blocks.Add(footerPara);

            return doc;
        }

        private void AddTableCell(TableRow row, string text, bool isHeader = false)
        {
            var cell = new TableCell(new Paragraph(new Run(text)))
            {
                Padding = new Thickness(5),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };

            if (isHeader)
            {
                cell.FontWeight = FontWeights.Bold;
            }

            row.Cells.Add(cell);
        }

        private void SaveAsPDF(FlowDocument document)
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    FileName = $"HoaDon_{_orderId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (saveDialog.ShowDialog() == true)
                {
                    MessageBox.Show($"Đã lưu PDF: {saveDialog.FileName}",
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu PDF: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}