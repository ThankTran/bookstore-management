using bookstore_Management.DTOs.Order.Requests;
using bookstore_Management.DTOs.Order.Responses; 
using bookstore_Management.DTOs.Book.Responses;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Core.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace bookstore_Management.Presentation.Views.Dialogs.Invoices
{
    public partial class CreateOrderBill : Window
    {
        // DataContext nội bộ (không tách file)
        private readonly CreateOrderBillState _vm = new CreateOrderBillState();

        // Danh sách sách để chọn (load từ BookService)
        private readonly IBookService _bookService;
        private List<BookDetailResponseDto> _allBooks = new List<BookDetailResponseDto>();

        public CreateOrderBill()
        {
            InitializeComponent();

            // Khởi tạo BookService giống SelectBookDialog (để tự load sách)
            var context = new BookstoreDbContext();
            var unitOfWork = new UnitOfWork(context);
            _bookService = new BookService(unitOfWork);

            DataContext = _vm;

            // Update tổng tiền khi add/remove
            _vm.OrderItems.CollectionChanged += (_, __) => RecalculateTotals();

            // Default
            _vm.CreatedDate = DateTime.Now;
            _vm.OrderId = GenerateTempOrderId();
            _vm.SubtotalDisplay = "0 ₫";
            _vm.TotalDisplay = "0 ₫";
        }

        #region Public methods to load data

        /// <summary>
        /// Caller nên gọi để nạp danh sách nhân viên (ItemsSource cbStaff)
        /// Mỗi item chỉ cần có Id + Display (hoặc dùng dynamic: item.Id, item.Display).
        /// </summary>
        public void LoadStaffs(IEnumerable staffs)
        {
            _vm.Staffs.Clear();
            if (staffs == null) return;

            foreach (var s in staffs)
            {
                // hỗ trợ cả object có { Id, Display } hoặc model { StaffId, FullName }...
                var opt = DisplayOption.FromUnknown(
                    s,
                    idCandidates: new[] { "Id", "StaffId", "StaffID" },
                    displayCandidates: new[] { "Display", "Name", "FullName", "StaffName" }
                );

                if (opt != null) _vm.Staffs.Add(opt);
            }

            if (_vm.SelectedStaff == null && _vm.Staffs.Any())
                _vm.SelectedStaff = _vm.Staffs.First();
        }

        /// <summary>
        /// Caller nên gọi để nạp danh sách khách hàng (ItemsSource cbCustomer)
        /// </summary>
        public void LoadCustomers(IEnumerable customers)
        {
            _vm.Customers.Clear();
            if (customers == null) return;

            foreach (var c in customers)
            {
                var opt = DisplayOption.FromUnknown(
                    c,
                    idCandidates: new[] { "Id", "CustomerId", "CustomerID" },
                    displayCandidates: new[] { "Display", "Name", "FullName", "CustomerName", "Phone" }
                );

                if (opt != null) _vm.Customers.Add(opt);
            }
        }

        /// <summary>
        /// Lấy DTO để service tạo đơn hàng (giống GetImportBillData()).
        /// Giảm giá nhập ở UI là VND (absolute) -> convert sang Discount rate (0..1) cho OrderService.
        /// </summary>
        public CreateOrderRequestDto GetOrderData()
        {
            var subtotal = _vm.OrderItems.Sum(x => x.Subtotal);
            var discountVnd = ParseDecimalSafe(tbDiscount.Text);
            if (discountVnd < 0) discountVnd = 0;
            if (discountVnd > subtotal) discountVnd = subtotal;

            var discountRate = subtotal <= 0 ? 0 : (discountVnd / subtotal); // 0..1

            var staffId = _vm.SelectedStaff?.Id;
            var customerId = _vm.SelectedCustomer?.Id; // optional

            return new CreateOrderRequestDto
            {
                StaffId = staffId,
                CustomerId = string.IsNullOrWhiteSpace(customerId) ? null : customerId,
                PaymentMethod = MapPaymentMethod(cbPaymentMethod.SelectedIndex),
                Discount = discountRate,
                Notes = string.IsNullOrWhiteSpace(tbNotes.Text) ? null : tbNotes.Text.Trim(),
                OrderDetails = _vm.OrderItems
                    .Where(x => !string.IsNullOrWhiteSpace(x.BookId) && x.Quantity > 0)
                    .Select(x => new OrderDetailCreateRequestDto
                    {
                        BookId = x.BookId,
                        Quantity = x.Quantity,
                        Notes = null
                    })
                    .ToList()
            };
        }
        #endregion

        #region Window events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBooksFromDatabase();
            RecalculateTotals();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => BtnCancel_Click(sender, e);

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.OrderItems.Any() || !string.IsNullOrWhiteSpace(tbNotes.Text) || ParseDecimalSafe(tbDiscount.Text) > 0)
            {
                var result = MessageBox.Show(
                    "Bạn có chắc muốn hủy? Tất cả dữ liệu đã nhập sẽ bị mất.",
                    "Xác nhận hủy",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No) return;
            }

            DialogResult = false;
            Close();
        }

        #endregion

        #region Button handlers

        private void BtnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            // Bạn có thể thay bằng dialog tạo khách hàng của dự án.
            MessageBox.Show(
                "Chức năng thêm khách hàng mới chưa được gắn dialog trong CreateOrderBill.\n" +
                "Bạn hãy mở dialog tạo khách hàng tại đây rồi reload lại danh sách Customers.",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void BtnAddBook_Click(object sender, RoutedEventArgs e)
        {
            if (_allBooks == null || _allBooks.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu sách để chọn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Mở picker “giống SelectBookDialog” nhưng viết code-behind, không tạo file XAML mới
            var picker = new QuickSelectBookForOrderDialog(_allBooks) { Owner = this };
            if (picker.ShowDialog() == true && picker.SelectedBook != null)
            {
                AddOrIncreaseBook(picker.SelectedBook, picker.SelectedQuantity);
            }
        }

        private void BtnRemoveBook_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is OrderBookItem item)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc muốn xóa sách '{item.BookName}' khỏi danh sách?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                item.PropertyChanged -= OrderItem_PropertyChanged;
                _vm.OrderItems.Remove(item);
                RecalculateTotals();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            var subtotal = _vm.OrderItems.Sum(x => x.Subtotal);
            var discountVnd = ParseDecimalSafe(tbDiscount.Text);
            if (discountVnd < 0) discountVnd = 0;
            if (discountVnd > subtotal) discountVnd = subtotal;

            var total = subtotal - discountVnd;

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn tạo hóa đơn với tổng thanh toán {total:N0} ₫?",
                "Xác nhận tạo hóa đơn",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            DialogResult = true;
            Close();
        }

        #endregion

        #region Input handlers

        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void Quantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Quantity TextBox bind trực tiếp vào item.Quantity, nhưng vì ItemsControl + TextBox
            // thường phát sinh trường hợp parse lỗi, ta ép lại an toàn ở đây.
            if (sender is TextBox tb && tb.DataContext is OrderBookItem item)
            {
                var q = ParseIntSafe(tb.Text);
                if (q <= 0) q = 1;
                if (q > 10000) q = 10000;

                // tránh vòng lặp TextChanged
                if (item.Quantity != q) item.Quantity = q;

                // đồng bộ text hiển thị (khi user xóa hết)
                if (tb.Text != q.ToString())
                {
                    var caret = tb.CaretIndex;
                    tb.Text = q.ToString();
                    tb.CaretIndex = Math.Min(caret, tb.Text.Length);
                }
            }
        }

        private void Discount_TextChanged(object sender, TextChangedEventArgs e)
        {
            RecalculateTotals();
        }

        #endregion

        #region ComboBox handlers

        private async void LoadBooksFromDatabase()
        {
            try
            {
                var result = await _bookService.GetAllBooksAsync();

                if (!result.IsSuccess || result.Data == null)
                {
                    MessageBox.Show(result.ErrorMessage ?? "Không tải được danh sách sách!",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    _allBooks = new List<BookDetailResponseDto>();
                    return;
                }

                // Bán hàng: lấy tất cả sách (không filter NXB)
                _allBooks = result.Data.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách sách: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                _allBooks = new List<BookDetailResponseDto>();
            }
        }

        private void AddOrIncreaseBook(BookDetailResponseDto book, int quantityToAdd)
        {
            if (book == null) return;

            if (quantityToAdd <= 0) quantityToAdd = 1;
            if (quantityToAdd > 10000) quantityToAdd = 10000;

            var existing = _vm.OrderItems.FirstOrDefault(x => x.BookId == book.BookId);
            if (existing != null)
            {
                existing.Quantity = Math.Min(10000, existing.Quantity + quantityToAdd);
                return;
            }

            // SalePrice từ dto (có thể null) -> nếu null thì cho 0, nhưng sẽ validate khi Save
            var salePrice = book.SalePrice ?? 0m;

            var item = new OrderBookItem
            {
                BookId = book.BookId,
                BookName = book.Name,
                SalePrice = salePrice,
                Quantity = quantityToAdd
            };

            item.PropertyChanged += OrderItem_PropertyChanged;
            _vm.OrderItems.Add(item);
            RecalculateTotals();
        }

        private void OrderItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OrderBookItem.Quantity) ||
                e.PropertyName == nameof(OrderBookItem.SalePrice) ||
                e.PropertyName == nameof(OrderBookItem.Subtotal))
            {
                RecalculateTotals();
            }
        }

        private void RecalculateTotals()
        {
            var subtotal = _vm.OrderItems.Sum(x => x.Subtotal);

            var discountVnd = ParseDecimalSafe(tbDiscount?.Text);
            if (discountVnd < 0) discountVnd = 0;
            if (discountVnd > subtotal) discountVnd = subtotal;

            _vm.SubtotalDisplay = $"{subtotal:N0} ₫";
            _vm.TotalDisplay = $"{(subtotal - discountVnd):N0} ₫";

            // Nếu user nhập vượt subtotal, ép lại
            if (tbDiscount != null)
            {
                var current = ParseDecimalSafe(tbDiscount.Text);
                if (current != discountVnd)
                {
                    tbDiscount.TextChanged -= Discount_TextChanged;
                    tbDiscount.Text = ((long)discountVnd).ToString();
                    tbDiscount.TextChanged += Discount_TextChanged;
                    tbDiscount.CaretIndex = tbDiscount.Text.Length;
                }
            }
        }

        #endregion

        #region Validation
        private bool ValidateForm()
        {
            // Validate Staff (bắt buộc)
            if (_vm.SelectedStaff == null || string.IsNullOrWhiteSpace(_vm.SelectedStaff.Id))
            {
                MessageBox.Show("Vui lòng chọn nhân viên!", "Lỗi nhập liệu",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cbStaff.Focus();
                return false;
            }

            // Validate OrderItems (bắt buộc)
            if (!_vm.OrderItems.Any())
            {
                MessageBox.Show("Vui lòng thêm ít nhất một sách vào danh sách!", "Lỗi nhập liệu",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Check for duplicates
            var duplicates = _vm.OrderItems
                .GroupBy(x => x.BookId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Any())
            {
                MessageBox.Show($"Có sách bị trùng lặp trong danh sách!\nVui lòng kiểm tra lại.",
                    "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validate each item
            foreach (var item in _vm.OrderItems)
            {
                if (string.IsNullOrWhiteSpace(item.BookId))
                {
                    MessageBox.Show("Có sách không hợp lệ (thiếu mã sách).", "Lỗi nhập liệu",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (item.Quantity <= 0)
                {
                    MessageBox.Show($"Số lượng sách '{item.BookName}' phải lớn hơn 0!", "Lỗi nhập liệu",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (item.Quantity > 10000)
                {
                    MessageBox.Show($"Số lượng sách '{item.BookName}' không được vượt quá 10,000!",
                        "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (item.SalePrice <= 0)
                {
                    MessageBox.Show($"Sách '{item.BookName}' chưa có giá bán hợp lệ!", "Lỗi nhập liệu",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (item.SalePrice > 10_000_000m)
                {
                    MessageBox.Show($"Giá bán sách '{item.BookName}' vượt quá giới hạn!", "Lỗi nhập liệu",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                // Check stock availability (if you have stock info in BookDetailResponseDto)
                // This would require passing stock data to OrderBookItem
                // if (item.AvailableStock < item.Quantity) { ... }
            }

            // Validate Discount VND
            var subtotal = _vm.OrderItems.Sum(x => x.Subtotal);
            var discountVnd = ParseDecimalSafe(tbDiscount.Text);

            if (discountVnd < 0)
            {
                MessageBox.Show("Giảm giá không được âm!", "Lỗi nhập liệu",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbDiscount.Focus();
                return false;
            }

            if (discountVnd > subtotal)
            {
                MessageBox.Show("Giảm giá không được lớn hơn tổng tiền hàng!", "Lỗi nhập liệu",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbDiscount.Focus();
                return false;
            }

            // Validate discount rate
            var discountRate = subtotal > 0 ? (discountVnd / subtotal) : 0;
            if (discountRate > 0.5m) // > 50%
            {
                var confirm = MessageBox.Show(
                    $"Giảm giá {discountRate:P0} ({discountVnd:N0} ₫) có vẻ cao bất thường.\n" +
                    "Bạn có chắc chắn muốn tiếp tục?",
                    "Xác nhận giảm giá cao",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (confirm != MessageBoxResult.Yes)
                {
                    tbDiscount.Focus();
                    return false;
                }
            }

            // Validate total amount
            var total = subtotal - discountVnd;

            if (total < 0)
            {
                MessageBox.Show("Tổng tiền không hợp lệ!", "Lỗi nhập liệu",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (total > 1_000_000_000m) // > 1 tỷ
            {
                var confirm = MessageBox.Show(
                    $"Tổng tiền hóa đơn rất lớn: {total:N0} ₫\n" +
                    $"Số loại sách: {_vm.OrderItems.Count}\n" +
                    $"Tổng số lượng: {_vm.OrderItems.Sum(x => x.Quantity):N0} cuốn\n\n" +
                    "Bạn có chắc chắn muốn tạo hóa đơn này?",
                    "Xác nhận hóa đơn lớn",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (confirm != MessageBoxResult.Yes)
                {
                    return false;
                }
            }

            // Validate payment method
            if (cbPaymentMethod.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn phương thức thanh toán!", "Lỗi nhập liệu",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cbPaymentMethod.Focus();
                return false;
            }

            return true;
        }
        #endregion

        #region Helper methods

        private static int ParseIntSafe(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            text = text.Trim().Replace(",", "");
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : 0;
        }

        private static decimal ParseDecimalSafe(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0m;
            text = text.Trim().Replace(",", "");
            return decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var v) ? v : 0m;
        }

        private static PaymentType MapPaymentMethod(int selectedIndex)
        {
            var values = Enum.GetValues(typeof(PaymentType)).Cast<PaymentType>().ToList();

            // an toàn: nếu enum của bạn đúng thứ tự 0..n theo UI thì dùng index
            if (selectedIndex >= 0 && selectedIndex < values.Count)
                return values[selectedIndex];

            return values.First(); // fallback
        }


        private static string GenerateTempOrderId()
        {
            // UI chỉ để hiển thị “đang tạo”. Id thật thường do service generate khi lưu.
            return $"(Tự động) {DateTime.Now:yyyyMMdd-HHmmss}";
        }

        #endregion

        #region DataContext class

        private class CreateOrderBillState : INotifyPropertyChanged
        {
            private string _orderId;
            private DateTime _createdDate;
            private string _subtotalDisplay;
            private string _totalDisplay;
            private DisplayOption _selectedStaff;
            private DisplayOption _selectedCustomer;

            public string OrderId
            {
                get => _orderId;
                set { _orderId = value; OnPropertyChanged(nameof(OrderId)); }
            }

            public DateTime CreatedDate
            {
                get => _createdDate;
                set { _createdDate = value; OnPropertyChanged(nameof(CreatedDate)); }
            }

            public ObservableCollection<DisplayOption> Staffs { get; } = new ObservableCollection<DisplayOption>();
            public ObservableCollection<DisplayOption> Customers { get; } = new ObservableCollection<DisplayOption>();

            public DisplayOption SelectedStaff
            {
                get => _selectedStaff;
                set { _selectedStaff = value; OnPropertyChanged(nameof(SelectedStaff)); }
            }

            public DisplayOption SelectedCustomer
            {
                get => _selectedCustomer;
                set { _selectedCustomer = value; OnPropertyChanged(nameof(SelectedCustomer)); }
            }

            public ObservableCollection<OrderBookItem> OrderItems { get; } = new ObservableCollection<OrderBookItem>();

            public string SubtotalDisplay
            {
                get => _subtotalDisplay;
                set { _subtotalDisplay = value; OnPropertyChanged(nameof(SubtotalDisplay)); }
            }

            public string TotalDisplay
            {
                get => _totalDisplay;
                set { _totalDisplay = value; OnPropertyChanged(nameof(TotalDisplay)); }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public class DisplayOption
        {
            public string Id { get; set; }
            public string Display { get; set; }

            public static DisplayOption FromUnknown(object obj, string[] idCandidates, string[] displayCandidates)
            {
                if (obj == null) return null;

                // nếu đã là DisplayOption
                if (obj is DisplayOption opt) return opt;

                var t = obj.GetType();

                string id = null;
                foreach (var p in idCandidates ?? Array.Empty<string>())
                {
                    var prop = t.GetProperty(p);
                    if (prop != null)
                    {
                        id = prop.GetValue(obj)?.ToString();
                        if (!string.IsNullOrWhiteSpace(id)) break;
                    }
                }

                string display = null;
                foreach (var p in displayCandidates ?? Array.Empty<string>())
                {
                    var prop = t.GetProperty(p);
                    if (prop != null)
                    {
                        display = prop.GetValue(obj)?.ToString();
                        if (!string.IsNullOrWhiteSpace(display)) break;
                    }
                }

                // fallback: nếu có Id mà thiếu Display thì dùng Id
                if (string.IsNullOrWhiteSpace(display) && !string.IsNullOrWhiteSpace(id))
                    display = id;

                if (string.IsNullOrWhiteSpace(id) && string.IsNullOrWhiteSpace(display))
                    return null;

                return new DisplayOption { Id = id ?? display, Display = display };
            }
        }

        public class OrderBookItem : INotifyPropertyChanged
        {
            private int _quantity;
            private decimal _salePrice;

            public string BookId { get; set; }
            public string BookName { get; set; }

            public int Quantity
            {
                get => _quantity;
                set
                {
                    var v = value;
                    if (v <= 0) v = 1;
                    if (v > 10000) v = 10000;

                    if (_quantity == v) return;
                    _quantity = v;
                    OnPropertyChanged(nameof(Quantity));
                    OnPropertyChanged(nameof(Subtotal));
                }
            }

            public decimal SalePrice
            {
                get => _salePrice;
                set
                {
                    if (_salePrice == value) return;
                    _salePrice = value;
                    OnPropertyChanged(nameof(SalePrice));
                    OnPropertyChanged(nameof(Subtotal));
                }
            }

            public decimal Subtotal => Quantity * SalePrice;

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Quick Select Book Dialog

        private class QuickSelectBookForOrderDialog : Window
        {
            private readonly List<BookDetailResponseDto> _all;
            private List<BookDetailResponseDto> _filtered;

            private TextBox _tbSearch;
            private ListBox _lb;
            private TextBox _tbQty;
            private TextBlock _tbSelected;

            public BookDetailResponseDto SelectedBook { get; private set; }
            public int SelectedQuantity { get; private set; } = 1;

            public QuickSelectBookForOrderDialog(List<BookDetailResponseDto> books)
            {
                _all = books ?? new List<BookDetailResponseDto>();
                _filtered = new List<BookDetailResponseDto>(_all);

                Title = "Chọn sách";
                Width = 620;
                Height = 520;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
                ResizeMode = ResizeMode.NoResize;
                WindowStyle = WindowStyle.ToolWindow;
                Background = Brushes.White;

                Content = BuildUI();
                Loaded += (_, __) => { _tbSearch.Focus(); RefreshList(); };
            }

            private UIElement BuildUI()
            {
                var root = new Grid { Margin = new Thickness(16) };
                root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // search
                root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // list
                root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // bottom

                // Search
                var spSearch = new StackPanel { Orientation = Orientation.Horizontal };
                _tbSearch = new TextBox
                {
                    Height = 32,
                    FontSize = 13,
                    Padding = new Thickness(10, 6, 10, 6),
                    Margin = new Thickness(0, 0, 8, 10)
                };
                _tbSearch.TextChanged += (_, __) =>
                {
                    var s = (_tbSearch.Text ?? "").Trim().ToLower();
                    _filtered = string.IsNullOrWhiteSpace(s)
                        ? new List<BookDetailResponseDto>(_all)
                        : _all.Where(b =>
                            ((b.BookId ?? "").ToLower().Contains(s)) ||
                            ((b.Name ?? "").ToLower().Contains(s)) ||
                            ((b.Author ?? "").ToLower().Contains(s)))
                          .ToList();
                    RefreshList();
                };

                var btnClear = new Button { Content = "Xóa", Height = 32, Width = 70 };
                btnClear.Click += (_, __) => _tbSearch.Text = "";
                spSearch.Children.Add(_tbSearch);
                spSearch.Children.Add(btnClear);
                Grid.SetRow(spSearch, 0);
                root.Children.Add(spSearch);

                // List
                _lb = new ListBox
                {
                    FontSize = 13,
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0")),
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(0, 0, 0, 10)
                };
                _lb.SelectionChanged += (_, __) =>
                {
                    SelectedBook = _lb.SelectedItem as BookDetailResponseDto;
                    _tbSelected.Text = SelectedBook == null ? "(Chưa chọn)" : $"{SelectedBook.BookId} - {SelectedBook.Name}";
                };
                Grid.SetRow(_lb, 1);
                root.Children.Add(_lb);

                // Bottom
                var bottom = new Grid();
                bottom.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                bottom.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                bottom.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                bottom.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                _tbSelected = new TextBlock
                {
                    Text = "(Chưa chọn)",
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#666666"))
                };
                Grid.SetColumn(_tbSelected, 0);
                bottom.Children.Add(_tbSelected);

                var spQty = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 0, 10, 0) };
                spQty.Children.Add(new TextBlock { Text = "SL:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 6, 0) });
                _tbQty = new TextBox
                {
                    Text = "1",
                    Width = 70,
                    Height = 30,
                    TextAlignment = TextAlignment.Center
                };
                _tbQty.PreviewTextInput += (s, e) => e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
                spQty.Children.Add(_tbQty);
                Grid.SetColumn(spQty, 1);
                bottom.Children.Add(spQty);

                var btnCancel = new Button { Content = "Hủy", Width = 90, Height = 32, Margin = new Thickness(0, 0, 8, 0) };
                btnCancel.Click += (_, __) => { DialogResult = false; Close(); };
                Grid.SetColumn(btnCancel, 2);
                bottom.Children.Add(btnCancel);

                var btnAdd = new Button { Content = "Thêm", Width = 90, Height = 32 };
                btnAdd.Click += (_, __) =>
                {
                    if (SelectedBook == null)
                    {
                        MessageBox.Show("Vui lòng chọn sách!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var q = ParseIntSafe(_tbQty.Text);
                    if (q <= 0) q = 1;
                    if (q > 10000) q = 10000;

                    SelectedQuantity = q;
                    DialogResult = true;
                    Close();
                };
                Grid.SetColumn(btnAdd, 3);
                bottom.Children.Add(btnAdd);

                Grid.SetRow(bottom, 2);
                root.Children.Add(bottom);

                return root;
            }

            private void RefreshList()
            {
                _lb.ItemsSource = null;
                _lb.ItemsSource = _filtered;
                _lb.DisplayMemberPath = "Name";
            }
        }
        #endregion
    }
}
