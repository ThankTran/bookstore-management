using bookstore_Management.Core.Enums;
using bookstore_Management.Models;
using bookstore_Management.Presentation.ViewModels;
using bookstore_Management.Presentation.Views;
using bookstore_Management.Presentation.Views.Information;
using bookstore_Management.Presentation.Views.Payment;
using bookstore_Management.Presentation.Views.Publishers;
using bookstore_Management.Presentation.Views.Users;
using bookstore_Management.Views.Books;
using bookstore_Management.Views.Customers;
using bookstore_Management.Presentation.Views.Statistics;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;
using bookstore_Management.Presentation.Views.Orders;

namespace bookstore_Management
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetClickedButtonColor(btnHome);
            LoadHomePage();
        }

        #region Navigation Helpers

        private void NavigateToView(UserControl view)
        {
            MainFrame.Content = view;
        }

        private void LoadHomePage()
        {
            var scope = App.Services.CreateScope();
            var view = scope.ServiceProvider.GetRequiredService<HomeView>();
            view.Unloaded += (_, __) => scope.Dispose();
            NavigateToView(view);
        }

        private void LoadCustomerListPage()
        {
            var scope = App.Services.CreateScope();
            var view = scope.ServiceProvider.GetRequiredService<CustomerListView>();
            
            view.CustomerSelected += (s, customer) =>
            {
                LoadCustomerDetailPage(customer);
            };
            
            view.Unloaded += (_, __) => scope.Dispose();
            NavigateToView(view);
        }

        private void LoadCustomerDetailPage(Customer customer)
        {
            var scope = App.Services.CreateScope();
            var view = scope.ServiceProvider.GetRequiredService<CustomerDetailView>();
            
            view.LoadCustomer(customer);
            
            view.ReturnToList += (s, e) =>
            {
                LoadCustomerListPage();
            };
            
            view.Unloaded += (_, __) => scope.Dispose();
            NavigateToView(view);
        }

        private void LoadBookListPage()
        {
            var scope = App.Services.CreateScope();
            var view = scope.ServiceProvider.GetRequiredService<BookListView>();
            view.Unloaded += (_, __) => scope.Dispose();
            NavigateToView(view);
        }

        private void LoadStatisticsPage()
        {
            var scope = App.Services.CreateScope();
            var view = scope.ServiceProvider.GetRequiredService<DashboardView>();
            view.Unloaded += (_, __) => scope.Dispose();
            NavigateToView(view);
        }

        private void LoadPaymentPage()
        {
            var scope = App.Services.CreateScope();
            var view = scope.ServiceProvider.GetRequiredService<PaymentView>();
            view.Unloaded += (_, __) => scope.Dispose();
            NavigateToView(view);
        }

        private void LoadPublisherPage()
        {
            var scope = App.Services.CreateScope();
            var view = scope.ServiceProvider.GetRequiredService<PublisherListView>();
            view.Unloaded += (_, __) => scope.Dispose();
            NavigateToView(view);
        }

        private void LoadStaffPage()
        {
            var scope = App.Services.CreateScope();
            var view = scope.ServiceProvider.GetRequiredService<StaffListView>();
            view.Unloaded += (_, __) => scope.Dispose();
            NavigateToView(view);
        }

        private void LoadInvoicePage()
        {
            var scope = App.Services.CreateScope();
            var view = scope.ServiceProvider.GetRequiredService<InvoiceView>();
            view.Unloaded += (_, __) => scope.Dispose();
            NavigateToView(view);
        }

        private void LoadAccountPage()
        {
            var scope = App.Services.CreateScope();
            var view = scope.ServiceProvider.GetRequiredService<AccountListView>();
            view.Unloaded += (_, __) => scope.Dispose();
            NavigateToView(view);
        }

        #endregion

        #region Window Control Events

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Normal
                ? WindowState.Maximized
                : WindowState.Normal;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion

        #region Button State Management

        public void ResetAllButtons()
        {
            btnHome.Tag = null;
            btnPayment.Tag = null;
            btnCustomer.Tag = null;
            btnPublisher.Tag = null;
            btnBills.Tag = null;
            btnBooks.Tag = null;
            btnStaffs.Tag = null;
            btnStatistics.Tag = null;
            btnAccount.Tag = null;
        }

        public void SetClickedButtonColor(Button btn)
        {
            ResetAllButtons();
            btn.Tag = "Selected";
        }

        #endregion

        #region Sidebar Menu Events

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetClickedButtonColor(btnHome);
                LoadHomePage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCustomerManagement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetClickedButtonColor(btnCustomer);
                LoadCustomerListPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBooks_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetClickedButtonColor(btnBooks);
                LoadBookListPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetClickedButtonColor(btnStatistics);
                LoadStatisticsPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnPayment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetClickedButtonColor(btnPayment);
                LoadPaymentPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnPublisher_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SessionModel.Role == 0)
                {
                    MessageBox.Show("Role không hợp lệ. Vui lòng chọn trang khác.");
                    return;
                }
                if (SessionModel.Role == UserRole.SalesStaff || SessionModel.Role == UserRole.CustomerManager)
                {
                    MessageBox.Show("Bạn không có quyền truy cập trang này");
                    return;
                }
                SetClickedButtonColor(btnPublisher);
                LoadPublisherPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnStaffs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SessionModel.Role == 0)
                {
                    MessageBox.Show("Role không hợp lệ. Vui lòng đăng nhập lại.");
                    return;
                }
                if (SessionModel.Role == UserRole.SalesStaff || 
                    SessionModel.Role == UserRole.InventoryManager || 
                    SessionModel.Role == UserRole.CustomerManager)
                {
                    MessageBox.Show("Bạn không có quyền truy cập trang này");
                    return;
                }
                SetClickedButtonColor(btnStaffs);
                LoadStaffPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBills_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetClickedButtonColor(btnBills);
                LoadInvoicePage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SessionModel.Role != UserRole.Administrator )
                {
                    MessageBox.Show("Bạn không có quyền truy cập trang này");
                    return;
                }
                SetClickedButtonColor(btnAccount);
                LoadAccountPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var scope = App.Services.CreateScope();
                var loginView = scope.ServiceProvider.GetRequiredService<LoginView>();
                loginView.Closed += (_, __) => scope.Dispose();
                
                Application.Current.MainWindow = loginView;
                loginView.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnI_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InforDialog infoView = new InforDialog
                {
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    Left = 15,
                    Top = 15
                };
                infoView.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}