using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace bookstore_Management.Views.Customers
{
    /// <summary>
    /// Interaction logic for CustomerDetailView.xaml
    /// </summary>
    public partial class CustomerDetailView : UserControl
    {
        public ObservableCollection<History> Histories { get; set; }

        public CustomerDetailView()
        {
            InitializeComponent();
            LoadSampleData();          // gọi để đổ dữ liệu demo
        }

        private void LoadSampleData()
        {
            Histories = new ObservableCollection<History>
            {
                new History { STT = 1, MaHD = "HD100", TongTien = "100.000.000", NgayMuaHang = "27/06/2006", GhiChu = "NULL" },
                new History { STT = 2, MaHD = "HD101", TongTien = "50.000.000",  NgayMuaHang = "10/07/2006", GhiChu = "NULL" },
                new History { STT = 3, MaHD = "HD102", TongTien = "75.000.000",  NgayMuaHang = "20/08/2006", GhiChu = "Khách VIP" },
            };

            dgHistories.ItemsSource = Histories;
        }

        public class History
        {
            public int STT { get; set; }
            public string MaHD { get; set; }
            public string TongTien { get; set; }
            public string NgayMuaHang { get; set; }
            public string GhiChu { get; set; }
        }
    }
}
