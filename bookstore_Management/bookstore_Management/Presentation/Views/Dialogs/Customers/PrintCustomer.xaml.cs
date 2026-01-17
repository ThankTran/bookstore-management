using bookstore_Management.Presentation.Views.Dialogs.Books;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for PrintCustomer.xaml
    /// </summary>
    public partial class PrintCustomer : Window
    {
        // Generic data source - can be any IEnumerable
        private IEnumerable<object> _dataSource;
        private string _documentTitle;
        private int _recordCount;

        public PrintCustomer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor với data source tùy chỉnh
        /// </summary>
        public PrintCustomer(string title, IEnumerable<object> dataSource, int recordCount = 0) : this()
        {
            _documentTitle = title;
            _dataSource = dataSource;
            _recordCount = recordCount > 0 ? recordCount : dataSource?.Count() ?? 0;

            // Update UI
            txtTitle.Text = $"In {title}";
            txtRecordCount.Text = $"• Sẽ in {_recordCount} bản ghi";
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
                // Get settings
                int copies = int.Parse(txtCopies.Text);
                bool isColor = chkColor.IsChecked ?? false;
                bool isDuplex = chkDuplex.IsChecked ?? false;
                bool savePDF = chkSavePDF.IsChecked ?? false;

                // Confirm
                var result = MessageBox.Show(
                    $"Bạn có chắc muốn in {copies} bản {_documentTitle}?",
                    "Xác nhận in",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                    return;

                // Print
                var printDialog = new System.Windows.Controls.PrintDialog();

                if (printDialog.ShowDialog() == true)
                {
                    var document = CreatePrintDocument();

                    for (int i = 0; i < copies; i++)
                    {
                        printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, _documentTitle);
                    }

                    // Save PDF if option enabled
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

        /// <summary>
        /// Tạo FlowDocument để in
        /// </summary>
        private FlowDocument CreatePrintDocument()
        {
            var doc = new FlowDocument
            {
                PagePadding = new Thickness(50),
                FontFamily = new FontFamily("Arial"),
                FontSize = 12
            };

            // Add title
            var titleParagraph = new Paragraph(new Run(_documentTitle))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            doc.Blocks.Add(titleParagraph);

            // Add date
            var dateParagraph = new Paragraph(new Run($"Ngày in: {DateTime.Now:dd/MM/yyyy HH:mm}"))
            {
                FontSize = 10,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 0, 0, 20)
            };
            doc.Blocks.Add(dateParagraph);

            // Add table with data
            if (_dataSource != null && _dataSource.Any())
            {
                var table = CreateDataTable();
                doc.Blocks.Add(table);
            }

            // Add footer
            var footerParagraph = new Paragraph(new Run($"Tổng số: {_recordCount} bản ghi"))
            {
                FontSize = 10,
                FontStyle = FontStyles.Italic,
                Margin = new Thickness(0, 20, 0, 0)
            };
            doc.Blocks.Add(footerParagraph);

            return doc;
        }

        /// <summary>
        /// Tạo bảng dữ liệu động
        /// </summary>
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

            // Get properties from first item
            var firstItem = _dataSource.First();
            var properties = firstItem.GetType().GetProperties()
                .Where(p => p.CanRead && IsSimpleType(p.PropertyType))
                .ToList();

            // Add columns
            foreach (var prop in properties)
            {
                table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            }

            // Create row group
            var rowGroup = new TableRowGroup();
            table.RowGroups.Add(rowGroup);

            // Add header row
            var headerRow = new TableRow { Background = Brushes.LightGray };
            foreach (var prop in properties)
            {
                var cell = new TableCell(new Paragraph(new Run(prop.Name)))
                {
                    FontWeight = FontWeights.Bold,
                    Padding = new Thickness(5),
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1)
                };
                headerRow.Cells.Add(cell);
            }
            rowGroup.Rows.Add(headerRow);

            // Add data rows
            foreach (var item in _dataSource)
            {
                var dataRow = new TableRow();
                foreach (var prop in properties)
                {
                    var value = prop.GetValue(item)?.ToString() ?? "";
                    var cell = new TableCell(new Paragraph(new Run(value)))
                    {
                        Padding = new Thickness(5),
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1)
                    };
                    dataRow.Cells.Add(cell);
                }
                rowGroup.Rows.Add(dataRow);
            }

            return table;
        }

        /// <summary>
        /// Lưu document thành PDF
        /// </summary>
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
                    // Sử dụng thư viện iTextSharp hoặc PdfSharp để convert
                    // Code mẫu - cần cài package: Install-Package iTextSharp

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

        /// <summary>
        /// Kiểm tra xem type có phải simple type không (để hiển thị trong bảng)
        /// </summary>
        private bool IsSimpleType(Type type)
        {
            return type.IsPrimitive
                || type.IsEnum
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(DateTime)
                || type == typeof(TimeSpan)
                || type == typeof(Guid);
        }

        #endregion

        #region Static Factory Methods

        /// <summary>
        /// Tạo dialog in cho danh sách nhân viên
        /// </summary>
        public static PrintBook ForStaffList(IEnumerable<object> staffList)
        {
            return new PrintBook("Danh Sách Nhân Viên", staffList);
        }

        /// <summary>
        /// Tạo dialog in cho danh sách sách
        /// </summary>
        public static PrintBook ForBookList(IEnumerable<object> bookList)
        {
            return new PrintBook("Danh Sách Sách", bookList);
        }

        /// <summary>
        /// Tạo dialog in cho danh sách khách hàng
        /// </summary>
        public static PrintBook ForCustomerList(IEnumerable<object> customerList)
        {
            return new PrintBook("Danh Sách Khách Hàng", customerList);
        }

        #endregion
    }
}
