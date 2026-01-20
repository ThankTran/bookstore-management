using bookstore_Management.Core.Enums;
using bookstore_Management.DTOs.Common.Reports;
using bookstore_Management.Models;
using bookstore_Management.Presentation.Views.Orders;
using bookstore_Management.Presentation.Views.Publishers;
using bookstore_Management.Presentation.Views.Statistics;
using bookstore_Management.Presentation.Views.Users;
using bookstore_Management.Services;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Views.Books;
using bookstore_Management.Views.Customers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace bookstore_Management.Presentation.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IReportService _reportService;
        private readonly NavigationService _navigationService;

        // --- 1. DATA DASHBOARD ---
        public decimal TodayRevenue { get; set; }
        public int TodayOrders { get; set; }
        public int NewCustomers { get; set; }
        public int LowStockBooks { get; set; }

        public string CurrentUsername => SessionModel.Username;
        public string CurrentRole => SessionModel.Role.ToString();

        public ObservableCollection<BookSalesReportResponseDto> TopSellingBooks { get; set; }

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
            var role = SessionModel.Role;

            MenuConfig["Dashboard"] = true;
            MenuConfig["Book"] = true;
            MenuConfig["Billing"] = true;

            MenuConfig["Stock"] = role != UserRole.CustomerManager;

            MenuConfig["User"] =
                role == UserRole.Administrator ||
                role == UserRole.SalesManager;

            MenuConfig["Customer"] = true;

            MenuConfig["Supplier"] =
                role == UserRole.Administrator ||
                role == UserRole.SalesManager ||
                role == UserRole.InventoryManager;

            MenuConfig["Report"] = true;

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
            NavigateDashboardCmd = new RelayCommand<object>(_ =>
                NavigateIfAllowed("Dashboard",
                    App.Services.GetRequiredService<DashboardView>()));

            NavigateBookCmd = new RelayCommand<object>(_ =>
                NavigateIfAllowed("Book",
                    App.Services.GetRequiredService<BookListView>()));

            NavigateBillingCmd = new RelayCommand<object>(_ =>
                NavigateIfAllowed("Billing",
                    App.Services.GetRequiredService<InvoiceView>()));

            NavigateUserCmd = new RelayCommand<object>(_ =>
                NavigateIfAllowed("User",
                    App.Services.GetRequiredService<AccountListView>()));

            NavigateCustomerCmd = new RelayCommand<object>(_ =>
                NavigateIfAllowed("Customer",
                    App.Services.GetRequiredService<CustomerListView>()));

            NavigateSupplierCmd = new RelayCommand<object>(_ =>
                NavigateIfAllowed("Supplier",
                    App.Services.GetRequiredService<PublisherListView>()));

            NavigateReportCmd = new RelayCommand<object>(_ =>
                NavigateIfAllowed("Report",
                    App.Services.GetRequiredService<DashboardView>()));
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