
using bookstore_Management.Data.Context;
using bookstore_Management.Models;
using bookstore_Management.Presentation.ViewModels;
using bookstore_Management.Presentation.Views;
using bookstore_Management.Presentation.Views.Information;
using bookstore_Management.Presentation.Views.Payment;
using bookstore_Management.Presentation.Views.Publishers;
using bookstore_Management.Presentation.Views.Users;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Views.Books;
using bookstore_Management.Views.Customers;
using System;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using bookstore_Management.Presentation.Views.Customers;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

namespace bookstore_Management
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Khởi tạo cửa sổ chính
        public MainWindow()
        {
            InitializeComponent();
            SetClickedButtonColor(btnHome);

            NavigateToView(new HomeView());

        }

        #region Navigation helpers

        /// <summary>
        /// Helper method để navigate đến view mới và dispose view cũ để tránh memory leak
        /// </summary>
        private void NavigateToView(UserControl newView)
        {
            // Dispose view cũ trước khi set view mới
            if (MainFrame.Content is IDisposable oldView)
            {
                try
                {
                    oldView.Dispose();
                }
                catch (Exception ex)
                {
                    // Log error nhưng không crash app
                    System.Diagnostics.Debug.WriteLine($"Error disposing view: {ex.Message}");
                }
            }
            MainFrame.Content = newView;
        }

        // Hiển thị danh sách khách hàng
        private void LoadCustomerListPage()
        {
            var customerListPage = new CustomerListView();

            customerListPage.CustomerSelected += (s, customer) =>
            {
                LoadCustomerDetailPage(customer);
            };

            NavigateToView(customerListPage);
        }

        // Hiển thị chi tiết một khách hàng được chọn
        private void LoadCustomerDetailPage(Customer customer)
        {
            var customerDetailView = new CustomerDetailView();

            customerDetailView.LoadCustomer(customer);

            customerDetailView.ReturnToList += (s, e) =>
            {
                LoadCustomerListPage();
            };

            MainFrame.Content = customerDetailView;
        }

        #endregion

        #region Window control events

        // Xử lý nút đóng cửa sổ
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Xử lý nút phóng to / khôi phục kích thước
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Normal
                ? WindowState.Maximized
                : WindowState.Normal;
        }

        // Xử lý nút thu nhỏ cửa sổ
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion

        #region menu click events
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

        #region Sidebar menu events

        // Xử lý click menu Trang chủ

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            SetClickedButtonColor(btnHome);
            NavigateToView(new HomeView());
        }

        // Xử lý click menu Quản lý khách hàng
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


        // Xử lý click menu Quản lý sách
        private void btnBooks_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetClickedButtonColor(btnBooks);
                NavigateToView(new BookListView());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Xử lý click menu Thống kê
        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetClickedButtonColor(btnStatistics);
                NavigateToView(new Presentation.Views.Statistics.DashboardView());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var loginView = new LoginView();
            Application.Current.MainWindow = loginView;
            loginView.Show();
            this.Close();
        }

        // Xử lý khi di chuột vào nút thông tin    

        private void btnI_Click(object sender, RoutedEventArgs e)
        {
            InforDialog infoView = new InforDialog
            {
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = 15,
                Top = 15
            };
            infoView.Show();
        }

        // Xử lý click menu Thanh toán
        private void btnPayment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetClickedButtonColor(btnPayment);
                NavigateToView(new Presentation.Views.Payment.PaymentView());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Xử lý click menu Quản lý nhà xuất bản
        private void btnPublisher_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetClickedButtonColor(btnPublisher);
                MainFrame.Content = new PublisherListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Xử lý click menu Quản lý nhân viên
        private void btnStaffs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetClickedButtonColor(btnStaffs);
                NavigateToView(new StaffListView());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Xử lý click menu Hóa đơn 
        private void btnBills_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetClickedButtonColor(btnBills);
                NavigateToView(new InvoiceView());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Xử lý click menu Tài khoản
        private void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetClickedButtonColor(btnAccount);
                MainFrame.Content = new AccountListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion


    }
}
