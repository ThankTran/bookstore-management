using bookstore_Management.Core.Enums;
using bookstore_Management.Services; // Nơi chứa SessionService
using bookstore_Management.DTOs.Common.Reports;
using bookstore_Management.Presentation.Views.Orders;
using bookstore_Management.Presentation.Views.Statistics;
// using bookstore_Management.Presentation.Views.Books; // Nhớ using các View khác của bạn
// using bookstore_Management.Presentation.Views.Staff;
using bookstore_Management.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using bookstore_Management.Views.Books;
using bookstore_Management.Presentation.Views.Users;
using bookstore_Management.Views.Customers;
using bookstore_Management.Presentation.Views.Publishers;

namespace bookstore_Management.Presentation.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        private readonly IReportService _reportService;
        private readonly NavigationService _navigationService;

        // --- 1. DATA DASHBOARD ---
        public decimal TodayRevenue { get; set; }
        public int TodayOrders { get; set; }
        public int NewCustomers { get; set; }
        public int LowStockBooks { get; set; }
        public ObservableCollection<BookSalesReportResponseDto> TopSellingBooks { get; set; }

        // --- 2. CONFIG PHÂN QUYỀN ---
        // Key: Tên chức năng (giống trong Excel), Value: True (Hiện) / False (Ẩn)
        public Dictionary<string, bool> MenuConfig { get; set; }

        // --- 3. COMMANDS (Đủ các trang) ---
        public ICommand NavigateDashboardCmd { get; set; } // Trang chủ
        public ICommand NavigateBookCmd { get; set; }      // Quản lý sách
        public ICommand NavigateBillingCmd { get; set; }   // Quản lý hóa đơn (Bán hàng)
        public ICommand NavigateStockCmd { get; set; }     // Quản lý kho
        public ICommand NavigateUserCmd { get; set; }      // Quản lý nhân viên
        public ICommand NavigateCustomerCmd { get; set; }  // Quản lý khách hàng
        public ICommand NavigateSupplierCmd { get; set; }  // Quản lý nhà cung cấp
        public ICommand NavigateReportCmd { get; set; }    // Báo cáo

        public MainViewModel(IReportService reportService)
        {
            _reportService = reportService;
            _navigationService = new NavigationService();

            // 1. Tải số liệu
            LoadDashboardData();

            // 2. Tải quyền hạn
            LoadPermissions();

            // 3. Khởi tạo Commands (Điều hướng)
            InitializeCommands();
        }

        private void LoadPermissions()
        {
            MenuConfig = new Dictionary<string, bool>();

            // Lấy UserRole hiện tại (Nếu chưa login thì mặc định null hoặc khách)
            // Lưu ý: Đảm bảo SessionService trả về đúng UserRole khớp với Enum mới
            var currentUser = SessionService.Instance.CurrentUser;
            var role = currentUser?.Role; // Giả sử thuộc tính trong DTO là UserRole

            // Nếu role null (chưa login), ẩn hết hoặc xử lý riêng. 
            // Ở đây mình giả định đã login.

            // --- LOGIC PHÂN QUYỀN THEO EXCEL ---

            // 1. Trang chủ / Dashboard: Tất cả đều có quyền Full Access
            MenuConfig["Dashboard"] = true;

            // 2. Quản lý sách: Tất cả đều được xem hoặc sửa (Full/Seen) -> Hiện Menu
            MenuConfig["Book"] = true;

            // 3. Quản lý hóa đơn (Bán hàng): Tất cả đều được xem hoặc sửa -> Hiện Menu
            MenuConfig["Billing"] = true;

            // 4. Quản lý kho (Stock)
            // - Administrator, SalesManager, SalesStaff, InventoryManager: Có quyền (Full/Seen)
            // - CustomerManager: No access
            MenuConfig["Stock"] = role != UserRole.CustomerManager;

            // 5. Quản lý nhân viên (User)
            // - Administrator: Full
            // - SalesManager: Seen
            // - Còn lại (SalesStaff, Inventory, CustomerMgr): No access
            MenuConfig["User"] = (role == UserRole.Administrator || role == UserRole.SalesManager);

            // 6. Quản lý khách hàng (Customer): Tất cả đều có quyền (Full/Seen)
            MenuConfig["Customer"] = true;

            // 7. Quản lý nhà cung cấp (Supplier)
            // - Administrator, SalesManager, InventoryManager: Có quyền
            // - SalesStaff, CustomerManager: No access
            bool canViewSupplier = role == UserRole.Administrator ||
                                   role == UserRole.SalesManager ||
                                   role == UserRole.InventoryManager;
            MenuConfig["Supplier"] = canViewSupplier;

            // 8. Báo cáo (Report): Tất cả đều có quyền (Full/Seen)
            MenuConfig["Report"] = true;

            // Cập nhật giao diện
            OnPropertyChanged(nameof(MenuConfig));
        }

        private void InitializeCommands()
        {
            // Helper để check quyền trước khi Navigate (Phòng hờ gọi code sai)
            void NavigateIfAllowed(string key, UserControl view)
            {
                if (MenuConfig.ContainsKey(key) && MenuConfig[key])
                {
                    _navigationService.Navigate?.Invoke(view);
                }
                else
                {
                    MessageBox.Show("Bạn không có quyền truy cập chức năng này!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            // Gán Command
            NavigateDashboardCmd = new RelayCommand<object>(p => NavigateIfAllowed("Dashboard", new DashboardView()));

            // Lưu ý: Thay new View() bằng các View thực tế của bạn
            NavigateBookCmd = new RelayCommand<object>(p => NavigateIfAllowed("Book", new BookListView())); // Tạm thay bằng InvoiceView để test, bạn nhớ đổi lại BookView

            NavigateBillingCmd = new RelayCommand<object>(p => NavigateIfAllowed("Billing", new InvoiceView()));

            NavigateUserCmd = new RelayCommand<object>(p => NavigateIfAllowed("User", new AccountListView())); // Thay bằng UserView

            NavigateCustomerCmd = new RelayCommand<object>(p => NavigateIfAllowed("Customer", new CustomerListView())); // Thay bằng CustomerView

            NavigateSupplierCmd = new RelayCommand<object>(p => NavigateIfAllowed("Supplier", new PublisherListView())); // Thay bằng SupplierView

            NavigateReportCmd = new RelayCommand<object>(p => NavigateIfAllowed("Report", new DashboardView())); // Thay bằng ReportView
        }

        private void LoadDashboardData()
        {
            try
            {
                var from = DateTime.Today.AddMonths(-1);
                var to = DateTime.Today.AddDays(1).AddTicks(-1);

                TodayRevenue = _reportService.GetTotalRevenue(from, to).Data;
                TodayOrders = _reportService.GetTotalOrderCount(from, to).Data;
                NewCustomers = _reportService.GetTotalCustomerCount(from, to).Data;

                var inventoryResult = _reportService.GetInventorySummary();
                LowStockBooks = (inventoryResult.IsSuccess && inventoryResult.Data != null)
                                ? inventoryResult.Data.OutOfStockCount : 0;

                var topBooksResult = _reportService.GetTopSellingBooks(from, to, 3);
                if (topBooksResult.IsSuccess)
                {
                    TopSellingBooks = new ObservableCollection<BookSalesReportResponseDto>(topBooksResult.Data);
                }

                // Notify changes
                OnPropertyChanged(nameof(TodayRevenue));
                OnPropertyChanged(nameof(TodayOrders));
                OnPropertyChanged(nameof(NewCustomers));
                OnPropertyChanged(nameof(LowStockBooks));
                OnPropertyChanged(nameof(TopSellingBooks));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu Dashboard: {ex.Message}");
            }
        }
    }

    public class NavigationService
    {
        public Action<UserControl> Navigate { get; set; }
    }
}