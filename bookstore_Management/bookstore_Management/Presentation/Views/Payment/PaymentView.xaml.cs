using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.DTOs.Order.Requests;
using bookstore_Management.DTOs.Customer.Requests;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Core.Enums;
using bookstore_Management.Presentation.Views.Dialogs.Payment;
using bookstore_Management.Presentation.Views.Dialogs.Customers;

namespace bookstore_Management.Presentation.Views.Payment
{
    public partial class PaymentView : UserControl, INotifyPropertyChanged, IDisposable
    {
        #region Fields & Services

        private readonly IOrderService _orderService;
        private readonly IBookService _bookService;
        private readonly ICustomerService _customerService;
        private readonly BookstoreDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private System.Threading.Timer _searchDebounceTimer;
        private const int SEARCH_DEBOUNCE_MS = 500;

        // Sample data - replace with actual data from services
        private ObservableCollection<ProductItem> _products;
        private ObservableCollection<CartItem> _cartItems;
        private ObservableCollection<CustomerOption> _customers;

        private string _searchText;
        private CustomerOption _selectedCustomer;
        private PaymentMethodOption _selectedPaymentMethod;
        private int _completedCount;
        private decimal _subtotal;
        private decimal _loyaltyPoints;
        private decimal _discount;
        private decimal _total;

        #endregion

        #region Properties

        public ObservableCollection<ProductItem> Products
        {
            get => _products;
            set { _products = value; OnPropertyChanged(nameof(Products)); }
        }

        public ObservableCollection<CartItem> CartItems
        {
            get => _cartItems;
            set { _cartItems = value; OnPropertyChanged(nameof(CartItems)); }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                
                _searchDebounceTimer?.Dispose();
                _searchDebounceTimer = new System.Threading.Timer(_ =>
                {
                    Application.Current.Dispatcher.InvokeAsync(new Action(async () =>
                    {
                        await FilterProductsAsync();
                    }));
                }, null, (int)SEARCH_DEBOUNCE_MS, System.Threading.Timeout.Infinite);
            }
        }

        public ObservableCollection<CustomerOption> Customers
        {
            get => _customers;
            set { _customers = value; OnPropertyChanged(nameof(Customers)); }
        }

        public CustomerOption SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged(nameof(SelectedCustomer));
                UpdateLoyaltyPoints();
            }
        }

        public ObservableCollection<PaymentMethodOption> PaymentMethods { get; set; }

        public PaymentMethodOption SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set
            {
                _selectedPaymentMethod = value;
                OnPropertyChanged(nameof(SelectedPaymentMethod));
            }
        }

        public int CompletedCount
        {
            get => _completedCount;
            set { _completedCount = value; OnPropertyChanged(nameof(CompletedCount)); }
        }

        public decimal Subtotal
        {
            get => _subtotal;
            set { _subtotal = value; OnPropertyChanged(nameof(Subtotal)); }
        }

        public decimal LoyaltyPoints
        {
            get => _loyaltyPoints;
            set { _loyaltyPoints = value; OnPropertyChanged(nameof(LoyaltyPoints)); }
        }

        public decimal Discount
        {
            get => _discount;
            set { _discount = value; OnPropertyChanged(nameof(Discount)); }
        }

        public decimal Total
        {
            get => _total;
            set { _total = value; OnPropertyChanged(nameof(Total)); }
        }

        public DateTime OrderDateTime { get; set; } = DateTime.Now;

        #endregion

        #region Commands

        public ICommand AddToCartCommand { get; }
        public ICommand RemoveFromCartCommand { get; }
        public ICommand CheckoutCommand { get; }
        public ICommand SaveDraftCommand { get; }
        public ICommand AddNoteCommand { get; }
        public ICommand EditSubtotalCommand { get; }
        public ICommand EditLoyaltyPointsCommand { get; }
        public ICommand EditDiscountCommand { get; }

        #endregion

        #region Constructor

        public PaymentView()
        {
            InitializeComponent();
            DataContext = this;

            // Initialize services
            var context = new BookstoreDbContext();
            var unitOfWork = new UnitOfWork(context);
            

            _orderService = new OrderService(unitOfWork);
            _bookService = new BookService(unitOfWork);
            _customerService = new CustomerService(unitOfWork);

            // Initialize commands
            AddToCartCommand = new RelayCommand<ProductItem>(AddToCart);
            RemoveFromCartCommand = new RelayCommand<CartItem>(RemoveFromCart);
            CheckoutCommand = new RelayCommand(async () => await CheckoutAsync(), CanCheckout);
            SaveDraftCommand = new RelayCommand(SaveDraft);
            AddNoteCommand = new RelayCommand(AddNote);
            EditSubtotalCommand = new RelayCommand(EditSubtotal);
            EditLoyaltyPointsCommand = new RelayCommand(EditLoyaltyPoints);
            EditDiscountCommand = new RelayCommand(EditDiscount);

            // Initialize data
            CartItems = new ObservableCollection<CartItem>();
            CartItems.CollectionChanged += (_, __) => RecalculateTotals();

            InitializePaymentMethods();
            _ = LoadProductsAsync();
            _ = LoadCustomersAsync();
        }

        #endregion

        #region Data Loading

        private void InitializePaymentMethods()
        {
            PaymentMethods = new ObservableCollection<PaymentMethodOption>
            {
                new PaymentMethodOption { Type = PaymentType.Cash, Display = "💵 Tiền mặt" },
                new PaymentMethodOption { Type = PaymentType.Card, Display = "💳 Quẹt thẻ" },
                new PaymentMethodOption { Type = PaymentType.BankTransfer, Display = "📱 Chuyển khoản" },
                new PaymentMethodOption { Type = PaymentType.DebitCard, Display = "👜 Thẻ tín dụng" }
            };

            SelectedPaymentMethod = PaymentMethods.First(); // Mặc định tiền mặt
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                var result = await _bookService.GetAllBooksAsync();

                if (result.IsSuccess && result.Data != null)
                {
                    Products = new ObservableCollection<ProductItem>(
                        result.Data
                            .Where(b => b.SalePrice.HasValue && b.SalePrice.Value > 0)
                            .Select(b => new ProductItem
                            {
                                Id = b.BookId,
                                Name = b.Name,
                                Price = b.SalePrice.Value,
                                Stock = b.StockQuantity
                            })
                    );
                }
                else
                {
                    Products = new ObservableCollection<ProductItem>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải sản phẩm: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Products = new ObservableCollection<ProductItem>();
            }
        }

        private async Task LoadCustomersAsync()
        {
            try
            {
                var result = await _customerService.GetAllCustomersAsync();

                if (result.IsSuccess && result.Data != null)
                {
                    var customerList = result.Data
                        .Select(c => new CustomerOption
                        {
                            Id = c.CustomerId,
                            Display = $"{c.Name} - {c.Phone}",
                            Name = c.Name,
                            Phone = c.Phone,
                            LoyaltyPoints = c.LoyaltyPoints
                        })
                        .ToList();

                    // Thêm option "Khách vãng lai"
                    customerList.Insert(0, new CustomerOption
                    {
                        Id = null,
                        Display = "Khách vãng lai",
                        Name = "Khách vãng lai",
                        Phone = "",
                        LoyaltyPoints = 0
                    });

                    Customers = new ObservableCollection<CustomerOption>(customerList);
                    SelectedCustomer = Customers.FirstOrDefault(); // Mặc định chọn "Khách vãng lai"
                }
                else
                {
                    Customers = new ObservableCollection<CustomerOption>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                // Fallback: chỉ có option khách vãng lai
                Customers = new ObservableCollection<CustomerOption>
                {
                    new CustomerOption
                    {
                        Id = null,
                        Display = "Khách vãng lai",
                        Name = "Khách vãng lai",
                        LoyaltyPoints = 0
                    }
                };
                SelectedCustomer = Customers.First();
            }
        }

        private async Task FilterProductsAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadProductsAsync();
                return;
            }

            var search = SearchText.ToLower();
            var result = await _bookService.SearchByNameAsync(search);

            if (result.IsSuccess && result.Data != null)
            {
                Products = new ObservableCollection<ProductItem>(
                    result.Data
                        .Where(b => b.SalePrice.HasValue && b.SalePrice.Value > 0)
                        .Select(b => new ProductItem
                        {
                            Id = b.BookId,
                            Name = b.Name,
                            Price = b.SalePrice.Value,
                            Stock = b.StockQuantity
                        })
                );
            }
        }

        private void UpdateLoyaltyPoints()
        {
            // Tự động cập nhật điểm thưởng khi chọn khách hàng
            if (SelectedCustomer != null && SelectedCustomer.Id != null)
            {
                // Có thể sử dụng điểm thưởng của khách hàng
                // LoyaltyPoints = SelectedCustomer.LoyaltyPoints; // Tùy business logic
            }
            else
            {
                // Khách vãng lai không có điểm
                LoyaltyPoints = 0;
            }

            RecalculateTotals();
        }

        #endregion

        #region Cart Operations

        private void AddToCart(ProductItem product)
        {
            if (product == null) return;

            // Check if already in cart
            var existing = CartItems.FirstOrDefault(x => x.ProductId == product.Id);

            if (existing != null)
            {
                // Increase quantity
                if (existing.Quantity + 1 > product.Stock)
                {
                    MessageBox.Show(
                        $"Không đủ hàng trong kho!\nTồn kho: {product.Stock}",
                        "Cảnh báo",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                existing.Quantity++;
            }
            else
            {
                // Add new item
                if (product.Stock <= 0)
                {
                    MessageBox.Show(
                        "Sản phẩm hết hàng!",
                        "Cảnh báo",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                var cartItem = new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = 1,
                    AvailableStock = product.Stock
                };

                cartItem.PropertyChanged += CartItem_PropertyChanged;
                CartItems.Add(cartItem);
            }

            RecalculateTotals();
        }

        private void RemoveFromCart(CartItem item)
        {
            if (item == null) return;

            var confirm = MessageBox.Show(
                $"Xóa '{item.ProductName}' khỏi giỏ hàng?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                item.PropertyChanged -= CartItem_PropertyChanged;
                CartItems.Remove(item);
                RecalculateTotals();
            }
        }

        private void CartItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CartItem.Quantity) ||
                e.PropertyName == nameof(CartItem.Total))
            {
                RecalculateTotals();
            }
        }

        #endregion

        #region Calculations

        private void RecalculateTotals()
        {
            Subtotal = CartItems.Sum(x => x.Total);
            CompletedCount = CartItems.Count;

            // Apply loyalty points and discount
            var afterPoints = Subtotal - LoyaltyPoints;
            if (afterPoints < 0) afterPoints = 0;

            var afterDiscount = afterPoints - Discount;
            if (afterDiscount < 0) afterDiscount = 0;

            Total = afterDiscount;
        }

        #endregion

        #region Command Handlers

        private async Task CheckoutAsync()
        {
            if (!CanCheckout()) return;

            try
            {
                //// Kiểm tra đã login chưa
                //if (!Core.Session.SessionManager.Instance.IsLoggedIn)
                //{
                //    MessageBox.Show(
                //        "Vui lòng đăng nhập để thực hiện thanh toán!",
                //        "Chưa đăng nhập",
                //        MessageBoxButton.OK,
                //        MessageBoxImage.Warning);
                //    return;
                //}

                // Create order DTO
                var dto = new CreateOrderRequestDto
                {
                    //StaffId = Core.Session.SessionManager.Instance.StaffId, // ✅ Lấy từ session
                    CustomerId = SelectedCustomer?.Id, // null nếu khách vãng lai
                    PaymentMethod = SelectedPaymentMethod?.Type ?? PaymentType.Cash, // ✅ Lấy từ selector
                    Discount = Subtotal > 0 ? Discount / Subtotal : 0,
                    Notes = null,
                    OrderDetails = CartItems.Select(x => new OrderDetailCreateRequestDto
                    {
                        BookId = x.ProductId,
                        Quantity = x.Quantity,
                        Notes = null
                    }).ToList()
                };

                // Create order
                var result = await _orderService.CreateOrderAsync(dto);

                if (!result.IsSuccess)
                {
                    MessageBox.Show(
                        result.ErrorMessage,
                        "Lỗi",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                // Get created order details
                var orderResult = await _orderService.GetOrderByIdAsync(result.Data);

                if (!orderResult.IsSuccess)
                {
                    MessageBox.Show(
                        "Tạo đơn thành công nhưng không thể tải chi tiết!",
                        "Cảnh báo",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    ClearCart();
                    return;
                }

                // Show payment dialog
                var payDialog = new Pay(orderResult.Data)
                {
                    Owner = Window.GetWindow(this)
                };

                payDialog.ShowDialog();

                // Clear cart after successful checkout
                ClearCart();

                MessageBox.Show(
                    $"Tạo đơn hàng thành công!\nMã đơn: {result.Data}",
                    "Thành công",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi thanh toán: {ex.Message}",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private bool CanCheckout()
        {
            return CartItems.Any();
        }

        private void SaveDraft()
        {
            MessageBox.Show(
                "Chức năng lưu nháp đang phát triển",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void AddNote()
        {
            // TODO: Show note dialog
            MessageBox.Show(
                "Chức năng ghi chú đang phát triển",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void EditSubtotal()
        {
            MessageBox.Show(
                "Tạm tính được tính tự động từ giỏ hàng",
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void EditLoyaltyPoints()
        {
            var input = Microsoft.VisualBasic.Interaction.InputBox(
                "Nhập điểm thưởng sử dụng:",
                "Điểm thưởng",
                LoyaltyPoints.ToString(),
                -1, -1);

            if (string.IsNullOrWhiteSpace(input)) return;

            if (decimal.TryParse(input, out var points))
            {
                if (points < 0)
                {
                    MessageBox.Show("Điểm thưởng không được âm!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (points > Subtotal)
                {
                    MessageBox.Show("Điểm thưởng không được lớn hơn tạm tính!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                LoyaltyPoints = points;
                RecalculateTotals();
            }
        }

        private void EditDiscount()
        {
            var input = Microsoft.VisualBasic.Interaction.InputBox(
                "Nhập giảm giá (VNĐ):",
                "Giảm giá",
                Discount.ToString(),
                -1, -1);

            if (string.IsNullOrWhiteSpace(input)) return;

            if (decimal.TryParse(input, out var discount))
            {
                if (discount < 0)
                {
                    MessageBox.Show("Giảm giá không được âm!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var afterPoints = Subtotal - LoyaltyPoints;
                if (discount > afterPoints)
                {
                    MessageBox.Show("Giảm giá không được lớn hơn tiền hàng sau khi trừ điểm!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Discount = discount;
                RecalculateTotals();
            }
        }

        #endregion

        #region Helper Methods

        private void ClearCart()
        {
            foreach (var item in CartItems.ToList())
            {
                item.PropertyChanged -= CartItem_PropertyChanged;
            }

            CartItems.Clear();
            LoyaltyPoints = 0;
            Discount = 0;
            RecalculateTotals();
        }

        #endregion

        #region Event Handlers

        private async void BtnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new AddCustomer
                {
                    Owner = Window.GetWindow(this)
                };

                if (dialog.ShowDialog() == true)
                {
                    // Validate form trước khi tạo
                    if (string.IsNullOrWhiteSpace(dialog.CustomerName) || string.IsNullOrWhiteSpace(dialog.Phone))
                    {
                        MessageBox.Show("Vui lòng nhập đầy đủ thông tin khách hàng!", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Tạo DTO từ dialog
                    var dto = new CreateCustomerRequestDto
                    {
                        Name = dialog.CustomerName,
                        Phone = dialog.Phone,
                        Email = dialog.Email,
                        Address = dialog.Address
                    };

                    // Gọi service để tạo khách hàng
                    var result = await _customerService.AddCustomerAsync(dto);

                    if (!result.IsSuccess)
                    {
                        MessageBox.Show($"Không thể thêm khách hàng: {result.ErrorMessage}", "Lỗi",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Reload danh sách khách hàng
                    await LoadCustomersAsync();

                    // Chọn khách hàng vừa tạo
                    var newCustomer = Customers.FirstOrDefault(c => c.Id == result.Data);
                    if (newCustomer != null)
                    {
                        SelectedCustomer = newCustomer;
                    }

                    MessageBox.Show($"Đã thêm khách hàng thành công!\nMã khách hàng: {result.Data}", "Thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm khách hàng: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        

        #endregion

        #region IDisposable

        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    _searchDebounceTimer?.Dispose();
                    _unitOfWork?.Dispose();
                    _context?.Dispose();
                }
                _disposed = true;
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Helper Classes

        public class ProductItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
        }

        public class CartItem : INotifyPropertyChanged
        {
            private int _quantity;

            public string ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal UnitPrice { get; set; }
            public int AvailableStock { get; set; }

            public int Quantity
            {
                get => _quantity;
                set
                {
                    if (_quantity == value) return;
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                    OnPropertyChanged(nameof(Total));
                }
            }

            public decimal Total => UnitPrice * Quantity;

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class CustomerOption
        {
            public string Id { get; set; }
            public string Display { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public decimal LoyaltyPoints { get; set; }
        }

        public class PaymentMethodOption
        {
            public PaymentType Type { get; set; }
            public string Display { get; set; }
        }

        #endregion
    }

    #region RelayCommand Implementation

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke((T)parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    #endregion
}