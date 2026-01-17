using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using bookstore_Management.DTOs.Publisher.Responses;

namespace bookstore_Management.Presentation.Views.Dialogs.Publishers
{
    /// <summary>
    /// Interaction logic for PrintPublisher.xaml
    /// </summary>
    public partial class PrintPublisher : Window
    {
        private IEnumerable<PublisherResponseDto> _dataSource;
        private string _documentTitle;
        private int _recordCount;

        public PrintPublisher()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor với data source
        /// </summary>
        public PrintPublisher(string title, IEnumerable<PublisherResponseDto> dataSource, int recordCount = 0) : this()
        {
            _documentTitle = title;
            _dataSource = dataSource;
            _recordCount = recordCount > 0 ? recordCount : dataSource?.Count() ?? 0;

            txtTitle.Text = $"In {title}";
            txtRecordCount.Text = $"• Sẽ in {_recordCount} nhà xuất bản";
        }

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
                int copies = int.Parse(txtCopies.Text);
                bool savePDF = chkSavePDF.IsChecked ?? false;

                var result = MessageBox.Show(
                    $"Bạn có chắc muốn in {copies} bản {_documentTitle}?",
                    "Xác nhận in",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                    return;

                var printDialog = new System.Windows.Controls.PrintDialog();

                if (printDialog.ShowDialog() == true)
                {
                    var document = CreatePrintDocument();

                    for (int i = 0; i < copies; i++)
                    {
                        printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, _documentTitle);
                    }

                    if (savePDF)
                    {
                        SaveAsPDF(document);
                    }

                    MessageBox.Show(
                        $"Đã in thành công {copies} bản!",
                        "Thành công",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

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

        private FlowDocument CreatePrintDocument()
        {
            var doc = new FlowDocument
            {
                PagePadding = new Thickness(50),
                FontFamily = new FontFamily("Arial"),
                FontSize = 12
            };

            var titleParagraph = new Paragraph(new Run(_documentTitle))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            doc.Blocks.Add(titleParagraph);

            var dateParagraph = new Paragraph(new Run($"Ngày in: {DateTime.Now:dd/MM/yyyy HH:mm}"))
            {
                FontSize = 10,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 0, 0, 20)
            };
            doc.Blocks.Add(dateParagraph);

            if (_dataSource != null && _dataSource.Any())
            {
                var table = CreateDataTable();
                doc.Blocks.Add(table);
            }

            var footerParagraph = new Paragraph(new Run($"Tổng số: {_recordCount} nhà xuất bản"))
            {
                FontSize = 10,
                FontStyle = FontStyles.Italic,
                Margin = new Thickness(0, 20, 0, 0)
            };
            doc.Blocks.Add(footerParagraph);

            return doc;
        }

        private Table CreateDataTable()
        {
            var table = new Table
            {
                CellSpacing = 0,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };

            if (_dataSource == null || !_dataSource.Any())
                return table;

            // Add columns: STT, Mã NXB, Tên NXB, Số điện thoại, Email
            table.Columns.Add(new TableColumn { Width = new GridLength(50) });  // STT
            table.Columns.Add(new TableColumn { Width = new GridLength(80) });  // Mã
            table.Columns.Add(new TableColumn { Width = new GridLength(150) }); // Tên
            table.Columns.Add(new TableColumn { Width = new GridLength(100) }); // SĐT
            table.Columns.Add(new TableColumn { Width = new GridLength(120) }); // Email

            var rowGroup = new TableRowGroup();
            table.RowGroups.Add(rowGroup);

            // Header row
            var headerRow = new TableRow { Background = Brushes.LightGray };

            AddTableCell(headerRow, "STT", true);
            AddTableCell(headerRow, "Mã NXB", true);
            AddTableCell(headerRow, "Tên nhà xuất bản", true);
            AddTableCell(headerRow, "Số điện thoại", true);
            AddTableCell(headerRow, "Email", true);

            rowGroup.Rows.Add(headerRow);

            // Data rows
            int stt = 1;
            foreach (var publisher in _dataSource)
            {
                var dataRow = new TableRow();

                AddTableCell(dataRow, stt.ToString());
                AddTableCell(dataRow, publisher.Id ?? "");
                AddTableCell(dataRow, publisher.Name ?? "");
                AddTableCell(dataRow, publisher.Phone ?? "");
                AddTableCell(dataRow, publisher.Email ?? "");

                rowGroup.Rows.Add(dataRow);
                stt++;
            }

            return table;
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
                    FileName = $"{_documentTitle}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    MessageBox.Show($"Đã lưu PDF: {saveDialog.FileName}", "Thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu PDF: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Static Factory Methods

        public static PrintPublisher ForPublisherList(IEnumerable<PublisherResponseDto> publisherList)
        {
            return new PrintPublisher("Danh Sách Nhà Xuất Bản", publisherList);
        }

        #endregion
    }
}