
using bookstore_Management.Core.Enums;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Models;
using bookstore_Management.Presentation.ViewModels;
using bookstore_Management.Presentation.Views;
using bookstore_Management.Presentation.Views.Information;
using bookstore_Management.Presentation.Views.Orders;
using bookstore_Management.Presentation.Views.Payment;
using bookstore_Management.Presentation.Views.Publishers;
using bookstore_Management.Presentation.Views.Users;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Views.Books;
using bookstore_Management.Views.Customers;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            MainFrame.Content = new HomeView();
            //mainframe.content = new homeview();
            //var context = new BookstoreDbContext();
            // var reportService = new ReportService(
            //     new OrderRepository(context),
            //     new OrderDetailRepository(context),
            //     new BookRepository(context),
            //     new CustomerRepository(context),
            //     new ImportBillRepository(context),
            //     new ImportBillDetailRepository(context)
            //     );
        }

        #region Navigation helpers

        // Hiển thị danh sách khách hàng
        private void LoadCustomerListPage()
        {
            var customerListPage = new CustomerListView();

            customerListPage.CustomerSelected += (s, customer) =>
            {
                LoadCustomerDetailPage(customer);
            };

            MainFrame.Content = customerListPage;
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
            MainFrame.Content = new HomeView();
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
                MainFrame.Content = new BookListView();
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
                MainFrame.Content = new Presentation.Views.Statistics.DashboardView();
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
                MainFrame.Content = new Presentation.Views.Payment.PaymentView();
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
                if (SessionModel.Role == 0)
                {
                    MessageBox.Show("Role không hợp lệ. Vui lòng chọn trang khác.");
                    return;
                }
                if (SessionModel.Role==UserRole.SalesStaff || SessionModel.Role==UserRole.CustomerManager)
                {
                    MessageBox.Show("Bạn không có quyền truy cập trang này");
                    return;
                }
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
                if (SessionModel.Role == 0)
                {
                    MessageBox.Show("Role không hợp lệ. Vui lòng đăng nhập lại.");
                    return;
                }
                if (SessionModel.Role == UserRole.SalesStaff || SessionModel.Role==UserRole.InventoryManager || SessionModel.Role==UserRole.CustomerManager)
                {
                    MessageBox.Show("Bạn không có quyền truy cập trang này");
                    return;
                }
                SetClickedButtonColor(btnStaffs);                
                MainFrame.Content = new StaffListView();
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
                MainFrame.Content = new InvoiceView();
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
