using bookstore_Management.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace bookstore_Management.Views.Customers
{
    /// <summary>
    /// Interaction logic for CustomerListView.xaml
    /// </summary>
    public partial class CustomerListView : UserControl
    {
        public CustomerListView()
        {
            InitializeComponent();
            LoadSampleData();

        }

        // Sua
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        // Xoa
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }
        public ObservableCollection<Customer> Customers { get; set; }
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
                new Customer { STT = 12, MaKH = "KH111", TenKH = "Nguyễn Thị Lan", HangThanhVien = "Đồng", SDT = "0934567890", DoanhThu = 450000000 }
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
