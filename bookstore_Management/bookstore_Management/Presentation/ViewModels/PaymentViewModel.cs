using bookstore_Management.Core.Enums;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.DTOs.Order.Requests;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using bookstore_Management.Data.Repositories.Interfaces;
using Microsoft.VisualBasic;
using bookstore_Management.Models;

namespace bookstore_Management.Presentation.ViewModels
{
    public class PaymentViewModel : BaseViewModel
    {
        #region Events (giữ nguyên ý tưởng của bạn)

        public event Action RequestOpenAddCustomerDialog;
        public event Action RequestOpenPayDialog;


        #endregion

        #region Services (giữ nguyên)

        private readonly IStaffService _staffService;

        private readonly IOrderService _orderService;
        private readonly IBookService _bookService;
        private readonly ICustomerService _customerService;

        #endregion

        #region Fields (giữ nguyên)



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

        #region Properties (GIỮ NGUYÊN)

        public string BillDate => DateTime.Now.ToString("dd/MM/yyyy");
        public string BillTime => DateTime.Now.ToString("HH:mm");

        public bool HasItemsInCart => CartItems != null && CartItems.Any();

        public ObservableCollection<ProductItem> Products
        {
            get => _products;
            set { _products = value; OnPropertyChanged(); }
        }

        public ObservableCollection<CartItem> CartItems
        {
            get => _cartItems;
            set { _cartItems = value; OnPropertyChanged(); }
        }

        public ObservableCollection<CustomerOption> Customers
        {
            get => _customers;
            set { _customers = value; OnPropertyChanged(); }
        }
        private ObservableCollection<StaffOption> _staffs;
        private StaffOption _selectedStaff;
        public ObservableCollection<StaffOption> Staffs
        {
            get => _staffs;
            set { _staffs = value; OnPropertyChanged(); }
        }

        public StaffOption SelectedStaff
        {
            get => _selectedStaff;
            set { _selectedStaff = value; OnPropertyChanged(); }
        }


        private ObservableCollection<PaymentMethodOption> _paymentMethods;
        public ObservableCollection<PaymentMethodOption> PaymentMethods
        {
            get => _paymentMethods;
            set { _paymentMethods = value; OnPropertyChanged(); }
        }


        public CustomerOption SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged();
                UpdateLoyaltyPoints();
            }
        }

        public PaymentMethodOption SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set { _selectedPaymentMethod = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterProducts();
            }
        }

        public int CompletedCount
        {
            get => _completedCount;
            set { _completedCount = value; OnPropertyChanged(); }
        }

        public decimal Subtotal
        {
            get => _subtotal;
            set
            {
                _subtotal = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DiscountAmount)); // 👈 BẮT BUỘC
            }
        }

        public decimal DiscountAmount => Subtotal - Total;

        public decimal LoyaltyPoints
        {
            get => _loyaltyPoints;
            set { _loyaltyPoints = value; OnPropertyChanged(); }
        }
        private string _loyaltyInput;
        public string LoyaltyInput
        {
            get => _loyaltyInput;
            set
            {
                _loyaltyInput = value;
                ParseLoyalty();
                OnPropertyChanged();
            }
        }


        public decimal Discount
        {
            get => _discount;
            set { _discount = value; OnPropertyChanged(); }
        }
        private string _discountInput;
        public string DiscountInput
        {
            get => _discountInput;
            set
            {
                _discountInput = value;
                ParseDiscount();
                OnPropertyChanged();
            }
        }

        public decimal Total
        {
            get => _total;
            set
            {
                _total = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DiscountAmount)); // 👈 BẮT BUỘC
            }
        }

        private bool _isEditingLoyalty;
        public bool IsEditingLoyalty
        {
            get => _isEditingLoyalty;
            set { _isEditingLoyalty = value; OnPropertyChanged(); }
        }

        private bool _isEditingSubtotal;
        public bool IsEditingSubtotal
        {
            get => _isEditingSubtotal;
            set { _isEditingSubtotal = value; OnPropertyChanged(); }
        }

        private bool _isEditingDiscount;
        public bool IsEditingDiscount
        {
            get => _isEditingDiscount;
            set { _isEditingDiscount = value; OnPropertyChanged(); }
        }

        #endregion

        #region Commands (GIỮ NGUYÊN)

        public ICommand AddCustomerCommand { get; }
        public ICommand AddToCartCommand { get; }
        public ICommand RemoveFromCartCommand { get; }
        public RelayCommand CheckoutCommand { get; }
        public ICommand EditLoyaltyPointsCommand { get; }
        public ICommand EditSubtotalCommand { get; }
        public ICommand EditDiscountCommand { get; }

        public ICommand FinishEditLoyaltyCommand { get; }
        public ICommand FinishEditSubtotalCommand { get; }
        public ICommand FinishEditDiscountCommand { get; }


        #endregion

        #region Constructor (GIỮ NGUYÊN LOGIC)

        public PaymentViewModel(IUnitOfWork unitOfWork)
        {
            
            _orderService = new OrderService(unitOfWork);
            _bookService = new BookService(unitOfWork);
            _customerService = new CustomerService(unitOfWork);
            _staffService = new StaffService(unitOfWork);


            // commands
            AddCustomerCommand = new RelayCommand(OpenAddCustomerDialog);
            AddToCartCommand = new RelayCommand<ProductItem>(AddToCart);
            RemoveFromCartCommand = new RelayCommand<CartItem>(RemoveFromCart);
            CheckoutCommand = new RelayCommand(() =>
            {
                RequestOpenPayDialog?.Invoke();
            }, CanCheckout);

            EditLoyaltyPointsCommand = new RelayCommand(() =>
            {
                string input = Interaction.InputBox("Điểm", "Điểm thưởng", "0");
                if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int i))
                {
                    LoyaltyPoints = i;
                }
                RecalculateTotals();

            });
            EditSubtotalCommand = new RelayCommand(() =>
            {
                string input = Interaction.InputBox("Giảm giá", "Chiết khấu", "0");
                if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int i))
                {
                    Subtotal = i;
                }
                RecalculateTotals();
            });
            EditDiscountCommand = new RelayCommand(() =>
            {
                string input = Interaction.InputBox("Nhập chiết khấu (%)", "Discount", Discount.ToString());

                if (string.IsNullOrWhiteSpace(input))
                    return;

                if (!decimal.TryParse(input, out var value))
                {
                    MessageBox.Show("Chiết khấu phải là số!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (value < 0 || value > 100)
                {
                    MessageBox.Show("Chiết khấu phải từ 0 đến 100%", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Discount = value;
                RecalculateTotals();
            });


            FinishEditLoyaltyCommand = new RelayCommand(() =>
            {
                IsEditingLoyalty = false;
                RecalculateTotals();
            });

            FinishEditSubtotalCommand = new RelayCommand(() =>
            {
                IsEditingSubtotal = false;
                RecalculateTotals();
            });

            FinishEditDiscountCommand = new RelayCommand(() =>
            {
                IsEditingDiscount = false;
                RecalculateTotals();
            });



            // data
            CartItems = new ObservableCollection<CartItem>();
            CartItems.CollectionChanged += (_, __) =>
            {
                OnPropertyChanged(nameof(HasItemsInCart));
                CheckoutCommand.NotifyCanExecuteChanged();
            };


            InitializePaymentMethods();
            LoadProducts();
            LoadCustomers();
            LoadStaffs();
        }

        #endregion

        #region Dialog trigger (CHUYỂN TỪ VIEW)

        private void OpenAddCustomerDialog()
        {
            RequestOpenAddCustomerDialog?.Invoke();
        }

        public void ReloadCustomersAfterAdd()
        {
            LoadCustomers();
            SelectedCustomer = Customers.LastOrDefault();
        }

        #endregion

        #region === TẤT CẢ HÀM CÒN LẠI ===

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

            SelectedPaymentMethod = PaymentMethods.First();
        }

        private void LoadProducts()
        {
            try
            {
                var result = _bookService.GetAllBooks();

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

        private void LoadCustomers()
        {
            try
            {
                var result = _customerService.GetAllCustomers();

                if (result.IsSuccess && result.Data != null)
                {
                    var list = result.Data
                        .Select(c => new CustomerOption
                        {
                            Id = c.CustomerId,
                            Display = $"{c.Name} - {c.Phone}",
                            Name = c.Name,
                            Phone = c.Phone,
                            LoyaltyPoints = c.LoyaltyPoints
                        })
                        .ToList();

                    list.Insert(0, new CustomerOption
                    {
                        Id = null,
                        Display = "Khách vãng lai",
                        Name = "Khách vãng lai",
                        Phone = "",
                        LoyaltyPoints = 0
                    });

                    Customers = new ObservableCollection<CustomerOption>(list);
                    SelectedCustomer = Customers.FirstOrDefault();
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

        private void LoadStaffs()
        {
            try
            {
                var result = _staffService.GetAllStaff();

                if (result.IsSuccess && result.Data != null)
                {
                    Staffs = new ObservableCollection<StaffOption>(
                        result.Data.Select(s => new StaffOption
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Display = $"{s.Name} ({s.Id})"
                        })
                    );

                    SelectedStaff = Staffs.FirstOrDefault();
                }
                else
                {
                    Staffs = new ObservableCollection<StaffOption>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải nhân viên: {ex.Message}");
                Staffs = new ObservableCollection<StaffOption>();
            }
        }


        private void FilterProducts()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadProducts();
                return;
            }

            var search = SearchText.ToLower();
            var result = _bookService.SearchByName(search);

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
            if (SelectedCustomer != null && SelectedCustomer.Id != null)
            {
                LoyaltyPoints = SelectedCustomer.LoyaltyPoints; // ✅ LẤY TỪ DB
            }
            else
            {
                LoyaltyPoints = 0;
            }

            RecalculateTotals();
        }


        #endregion

        #region Cart Operations

        private void AddToCart(ProductItem product)
        {
            if (product == null) return;

            var existing = CartItems.FirstOrDefault(x => x.ProductId == product.Id);

            if (existing != null)
            {
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
                if (product.Stock <= 0)
                {
                    MessageBox.Show(
                        "Sản phẩm hết hàng!",
                        "Cảnh báo",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                var item = new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = 1,
                    AvailableStock = product.Stock
                };

                item.PropertyChanged += CartItem_PropertyChanged;
                CartItems.Add(item);
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

        private void ParseDiscount()
        {
            if (!decimal.TryParse(DiscountInput, out var value))
                value = 0;

            if (value < 0)
                value = 0;

            if (value > 100)
                value = 100;

            Discount = value;
            RecalculateTotals();
        }

        private void ParseLoyalty()
        {
            if (!decimal.TryParse(LoyaltyInput, out var value))
                value = 0;

            if (value < 0)
                value = 0;

            if (value > Subtotal)
                value = Subtotal;

            LoyaltyPoints = value;
            RecalculateTotals();
        }

        public void ConfirmCheckout()
        {
            Checkout();
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

            var afterPoints = Subtotal - LoyaltyPoints;
            if (afterPoints < 0) afterPoints = 0;

            var afterDiscount = afterPoints - (afterPoints * Discount / 100);
            if (afterDiscount < 0) afterDiscount = 0;

            Total = afterDiscount;

            OnPropertyChanged(nameof(DiscountAmount));
        }




        #endregion

        #region Checkout

        private void Checkout()
        {
            if (!CartItems.Any())
            {
                MessageBox.Show("Đơn hàng phải có ít nhất 1 sách");
                return;
            }

            try
            {
                var orderDetails = CartItems.Select(item => new OrderDetailCreateRequestDto
                {
                    BookId = item.ProductId,
                    Quantity = item.Quantity,
                    //Notes = item.UnitPrice,
                }).ToList();

                var dto = new CreateOrderRequestDto
                {
                    CustomerId = SelectedCustomer?.Id,
                    StaffId = SelectedStaff?.Id,
                    PaymentMethod = SelectedPaymentMethod?.Type ?? PaymentType.Cash,
                    Discount = Subtotal > 0 ? Discount / Subtotal : 0,
                    Notes = null,
                    OrderDetails = orderDetails // ✅ DÒNG QUYẾT ĐỊNH
                };

                var result = _orderService.CreateOrder(dto);

                if (!result.IsSuccess)
                {
                    MessageBox.Show(result.ErrorMessage, "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

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
            return HasItemsInCart;
        }


        #endregion

        #region Helpers

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
            CheckoutCommand.NotifyCanExecuteChanged();

        }

        #endregion

        #endregion
    }


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
    public class StaffOption
    {
        public string Id { get; set; }
        public string Display { get; set; }   // để bind ComboBox
        public string Name { get; set; }
    }


}
