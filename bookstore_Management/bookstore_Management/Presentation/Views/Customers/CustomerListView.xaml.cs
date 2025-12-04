using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace bookstore_Management.Views.Customers
{
    /// <summary>
    /// Interaction logic for CustomerListView.xaml
    /// </summary>
    public partial class CustomerListView : UserControl
    {
        public ObservableCollection<Customer> Customers { get; set; }
        public event EventHandler<Customer> CustomerSelected;

        // Khởi tạo danh sách khách hàng mẫu
        public CustomerListView()
        {
            InitializeComponent();
            LoadSampleData();
        }

        // Xử lý click nút thêm khách hàng
        private void btnAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        // Xử lý click nút sửa trong từng dòng
        private void btnEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        // Xử lý click nút xóa trong từng dòng
        private void btnDelete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        // Xử lý thay đổi lựa chọn khách hàng (dùng khi cần mở rộng)
        private void dgCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        // Xử lý double-click trên một dòng để mở màn chi tiết khách hàng
        private void dgCustomers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgCustomers.SelectedItem is Customer selectedCustomer)
            {
                CustomerSelected?.Invoke(this, selectedCustomer);
            }
        }

        // Tải dữ liệu khách hàng mẫu cho DataGrid
        private void LoadSampleData()
        {
            Customers = new ObservableCollection<Customer>
            {
                new Customer { STT = 1, MaKH = "KH100", TenKH = "Trần Thị Hồng Thanh", HangThanhVien = "Kim Cương", SDT = "0593793875", DoanhThu = 2760000000 },
                new Customer { STT = 2, MaKH = "KH101", TenKH = "Nguyễn Ái My", HangThanhVien = "Bạch Kim", SDT = "0861278318", DoanhThu = 500000000 },
                new Customer { STT = 3, MaKH = "KH102", TenKH = "Phạm Hoàng Gia Hiển", HangThanhVien = "Vàng", SDT = "0826437621", DoanhThu = 1500000000 },
                new Customer { STT = 4, MaKH = "KH103", TenKH = "Phạm Nguyên Gia Huy", HangThanhVien = "Kim Cương", SDT = "0782648645", DoanhThu = 700000000 },
                new Customer { STT = 5, MaKH = "KH104", TenKH = "Nguyễn Khánh Vi", HangThanhVien = "Đồng", SDT = "0173288246", DoanhThu = 300000000 },
                new Customer { STT = 6, MaKH = "KH105", TenKH = "Nguyễn Quang Nghĩa", HangThanhVien = "Bạc", SDT = "0861278318", DoanhThu = 2760000000 },
                new Customer { STT = 7, MaKH = "KH106", TenKH = "Võ Nguyên Khoa", HangThanhVien = "Bạc", SDT = "0826437621", DoanhThu = 500000000 },
                new Customer { STT = 8, MaKH = "KH107", TenKH = "Nguyễn Vũ Phúc", HangThanhVien = "Đồng", SDT = "0782648645", DoanhThu = 1500000000 },
                new Customer { STT = 9, MaKH = "KH108", TenKH = "Nguyễn Ngọc Hân", HangThanhVien = "Bạch Kim", SDT = "0173288246", DoanhThu = 700000000 },
                new Customer { STT = 10, MaKH = "KH109", TenKH = "Lê Minh Tuấn", HangThanhVien = "Vàng", SDT = "0912345678", DoanhThu = 1200000000 },
                new Customer { STT = 11, MaKH = "KH110", TenKH = "Trần Văn Nam", HangThanhVien = "Bạc", SDT = "0923456789", DoanhThu = 800000000 },
                new Customer { STT = 12, MaKH = "KH111", TenKH = "Nguyễn Thị Lan", HangThanhVien = "Đồng", SDT = "0934567890", DoanhThu = 450000000 },
            };

            dgCustomers.ItemsSource = Customers;
        }
    }

    public class Customer
    {
        public int STT { get; set; }
        public string MaKH { get; set; }
        public string TenKH { get; set; }
        public string HangThanhVien { get; set; }
        public string SDT { get; set; }
        public long DoanhThu { get; set; }
    }
}
