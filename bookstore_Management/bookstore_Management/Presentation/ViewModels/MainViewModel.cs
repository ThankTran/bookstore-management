using bookstore_Management.Core.Enums;
using bookstore_Management.Services;
using bookstore_Management.DTOs.Common.Reports;
using bookstore_Management.Presentation.Views.Statistics;
using bookstore_Management.Services.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using bookstore_Management.Presentation.Views.Customers;
using bookstore_Management.Presentation.Views.Payment;
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
        public Dictionary<string, bool> MenuConfig { get; set; }

        // --- 3. COMMANDS ---
        public ICommand NavigateDashboardCmd { get; set; }
        public ICommand NavigateBookCmd { get; set; }
        public ICommand NavigateBillingCmd { get; set; }
        public ICommand NavigateStockCmd { get; set; }
        public ICommand NavigateUserCmd { get; set; }
        public ICommand NavigateCustomerCmd { get; set; }
        public ICommand NavigateSupplierCmd { get; set; }
        public ICommand NavigateReportCmd { get; set; }

        public MainViewModel(IReportService reportService)
        {
            _reportService = reportService;
            _navigationService = new NavigationService();

            LoadDashboardData();
            LoadPermissions();
            InitializeCommands();
        }

        private void LoadPermissions()
        {
            MenuConfig = new Dictionary<string, bool>();
            var currentUser = SessionService.Instance.CurrentUser;
            var role = currentUser?.Role;

            MenuConfig["Dashboard"] = true;
            MenuConfig["Book"] = true;
            MenuConfig["Billing"] = true;
            MenuConfig["Stock"] = role != UserRole.CustomerManager;
            MenuConfig["User"] = (role == UserRole.Administrator || role == UserRole.SalesManager);
            MenuConfig["Customer"] = true;

            bool canViewSupplier = role == UserRole.Administrator ||
                                   role == UserRole.SalesManager ||
                                   role == UserRole.InventoryManager;
            MenuConfig["Supplier"] = canViewSupplier;

            MenuConfig["Report"] = true;

            OnPropertyChanged(nameof(MenuConfig));
        }

        private void InitializeCommands()
        {
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

            NavigateDashboardCmd = new AsyncRelayCommand<object>(async p => await Task.Run(() => NavigateIfAllowed("Dashboard", new DashboardView())));
            NavigateBookCmd = new AsyncRelayCommand<object>(async p => await Task.Run(() => NavigateIfAllowed("Book", new BookListView())));
            NavigateBillingCmd = new AsyncRelayCommand<object>(async p => await Task.Run(() => NavigateIfAllowed("Billing", new InvoiceView())));
            NavigateUserCmd = new AsyncRelayCommand<object>(async p => await Task.Run(() => NavigateIfAllowed("User", new AccountListView())));
            NavigateCustomerCmd = new AsyncRelayCommand<object>(async p => await Task.Run(() => NavigateIfAllowed("Customer", new CustomerListView())));
            NavigateSupplierCmd = new AsyncRelayCommand<object>(async p => await Task.Run(() => NavigateIfAllowed("Supplier", new PublisherListView())));
            NavigateReportCmd = new AsyncRelayCommand<object>(async p => await Task.Run(() => NavigateIfAllowed("Report", new DashboardView())));
        }

        private async Task LoadDashboardData()
        {
            try
            {
                var from = DateTime.Today.AddMonths(-1);
                var to = DateTime.Today.AddDays(1).AddTicks(-1);

                var revenue = await _reportService.GetTotalRevenueAsync(from, to);
                var orders = await _reportService.GetTotalOrderCountAsync(from, to);
                var cus = await _reportService.GetTotalCustomerCountAsync(from, to);

                var inventoryResult = await _reportService.GetInventorySummaryAsync();
                LowStockBooks = (inventoryResult.IsSuccess && inventoryResult.Data != null)
                                ? inventoryResult.Data.OutOfStockCount : 0;

                var topBooksResult =await _reportService.GetTopSellingBooksAsync(from, to, 3);
                if (topBooksResult.IsSuccess)
                {
                    TopSellingBooks = new ObservableCollection<BookSalesReportResponseDto>(topBooksResult.Data);
                }

                TodayRevenue = revenue.Data;
                TodayOrders = orders.Data;
                NewCustomers = cus.Data;
                
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
