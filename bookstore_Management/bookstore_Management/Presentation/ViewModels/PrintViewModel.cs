using bookstore_Management.Models; // Namespace chứa PrintColumnDef
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection; // Cần thiết để lấy dữ liệu động (Reflection)
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace bookstore_Management.Presentation.ViewModels // Đã sửa lại cho ngắn gọn và đúng chuẩn chung
{
    public class PrintViewModel : BaseViewModel
    {
        // --- DATA ---
        private readonly IEnumerable _dataList;
        private readonly List<PrintColumnDef> _columns;
        private readonly string _reportTitle;

        // --- BINDING PROPERTIES ---
        private int _copies = 1;
        public int Copies
        {
            get => _copies;
            set { if (value > 0) { _copies = value; OnPropertyChanged(); } }
        }

        private bool _isPortrait = true;
        public bool IsPortrait
        {
            get => _isPortrait;
            set { _isPortrait = value; OnPropertyChanged(); }
        }

        private string _paperSize = "A4";
        public string PaperSize
        {
            get => _paperSize;
            set { _paperSize = value; OnPropertyChanged(); }
        }

        // --- COMMANDS ---
        public ICommand IncreaseCopyCommand { get; set; }
        public ICommand DecreaseCopyCommand { get; set; }

        // --- CONSTRUCTOR (ĐÃ SỬA LỖI) ---
        public PrintViewModel(IEnumerable data, List<PrintColumnDef> columns, string title)
        {
            _dataList = data;
            _columns = columns;
            _reportTitle = title;

            // Khởi tạo Command (Giả sử bạn đã có class RelayCommand trong BaseViewModel hoặc Helpers)
            IncreaseCopyCommand = new RelayCommand<object>((p) => Copies++, (p) => true);
            DecreaseCopyCommand = new RelayCommand<object>((p) => Copies--, (p) => true);
        }

        // --- CORE LOGIC: TẠO DOCUMENT ---
        public FlowDocument CreateDocument()
        {
            // 1. Tính kích thước giấy
            double width = 794; double height = 1123; // Mặc định A4 (96 DPI)

            if (PaperSize.Contains("A5")) { width = 559; height = 794; }
            if (PaperSize.Contains("Letter")) { width = 816; height = 1056; }

            if (!IsPortrait) (width, height) = (height, width);

            FlowDocument doc = new FlowDocument
            {
                PageWidth = width,
                PageHeight = height,
                ColumnWidth = double.PositiveInfinity,
                // Quan trọng: Để nội dung không bị chia thành nhiều cột báo chí
                PagePadding = new Thickness(40),
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 13
            };

            // 2. Tiêu đề Báo cáo
            doc.Blocks.Add(new Paragraph(new Run(_reportTitle.ToUpper()))
            {
                FontSize = 22,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20),
                Foreground = (Brush)new BrushConverter().ConvertFrom("#2196F3") // Xanh dương
            });

            // 3. Tạo Bảng dữ liệu
            Table table = new Table { CellSpacing = 0, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };

            // 3a. Định nghĩa độ rộng cột
            foreach (var col in _columns)
            {
                // GridUnitType.Star giúp chia tỷ lệ % độ rộng cột
                table.Columns.Add(new TableColumn { Width = new GridLength(col.WidthStar, GridUnitType.Star) });
            }

            // 3b. Header Row (Dòng tiêu đề bảng)
            TableRowGroup headerGroup = new TableRowGroup();
            TableRow headerRow = new TableRow { Background = (Brush)new BrushConverter().ConvertFrom("#E3F2FD") };
            foreach (var col in _columns)
            {
                headerRow.Cells.Add(CreateCell(col.Header, true));
            }
            headerGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerGroup);

            // 3c. Data Rows (Dữ liệu từ List)
            TableRowGroup dataGroup = new TableRowGroup();
            if (_dataList != null)
            {
                int i = 0;
                foreach (var item in _dataList)
                {
                    TableRow row = new TableRow();
                    // Hiệu ứng Zebra (sọc dưa): Dòng lẻ màu xám nhạt cho dễ nhìn
                    if (i % 2 != 0) row.Background = Brushes.WhiteSmoke;

                    Type type = item.GetType();

                    foreach (var col in _columns)
                    {
                        // Tìm property có tên trùng với col.PropertyPath (Ví dụ: "Title", "Price")
                        var prop = type.GetProperty(col.PropertyPath);

                        // Lấy giá trị
                        var val = prop?.GetValue(item);

                        // Format giá trị hiển thị
                        string txt = FormatValue(val);

                        row.Cells.Add(CreateCell(txt, false));
                    }
                    dataGroup.Rows.Add(row);
                    i++;
                }
            }
            table.RowGroups.Add(dataGroup);
            doc.Blocks.Add(table);

            // 4. Footer (Chữ ký/Ngày tháng)
            doc.Blocks.Add(new Paragraph(new Run($"Ngày in: {DateTime.Now:dd/MM/yyyy HH:mm}"))
            {
                FontSize = 10,
                FontStyle = FontStyles.Italic,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 10, 0, 0)
            });

            return doc;
        }

        // Hàm phụ trợ tạo ô (Cell)
        private TableCell CreateCell(string text, bool isHeader)
        {
            return new TableCell(new Paragraph(new Run(text)) { Padding = new Thickness(2) })
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0, 0, 0.5, 0.5), // Viền mỏng bên phải và dưới
                Padding = new Thickness(5),
                FontWeight = isHeader ? FontWeights.Bold : FontWeights.Normal,
                // Căn giữa theo chiều dọc
                TextAlignment = isHeader ? TextAlignment.Center : (IsNumeric(text) ? TextAlignment.Right : TextAlignment.Left)
            };
        }

        // Hàm kiểm tra xem chuỗi có phải số/tiền tệ không để căn lề phải cho đẹp
        private bool IsNumeric(string text)
        {
            return text.EndsWith("đ") || double.TryParse(text, out _);
        }

        // Hàm định dạng dữ liệu hiển thị
        private string FormatValue(object val)
        {
            if (val == null) return "";
            if (val is DateTime dt) return dt.ToString("dd/MM/yyyy");
            // Format tiền tệ Việt Nam
            if (val is decimal dec) return dec.ToString("N0") + " đ";
            if (val is double dub) return dub.ToString("N0");
            if (val is int i) return i.ToString("N0");
            return val.ToString();
        }
    }
}