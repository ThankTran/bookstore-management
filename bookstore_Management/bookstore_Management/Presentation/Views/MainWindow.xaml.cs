using bookstore_Management.Views.Books;
using bookstore_Management.Views.Customers;
using System;
using System.Windows;

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
            LoadHomePage();
        }

        #region Navigation helpers

        // Hiển thị trang chủ mặc định
        private void LoadHomePage()
        {
            MainFrame.Content = null;
        }

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

        #region Sidebar menu events

        // Xử lý click menu Trang chủ
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            LoadHomePage();
        }

        // Xử lý click menu Quản lý khách hàng
        private void btnCustomerManagement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadCustomerListPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Xử lý click menu Hóa đơn (chưa cài đặt nội dung)
        private void btnBills_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Mở màn hình quản lý hóa đơn khi bạn xây dựng view tương ứng
        }

        // Xử lý click menu Quản lý sách
        private void btnBooks_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainFrame.Content = new BookListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
