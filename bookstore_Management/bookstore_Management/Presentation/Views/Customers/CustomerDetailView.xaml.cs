using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace bookstore_Management.Views.Customers
{
    public partial class CustomerDetailView : UserControl
    {
        public event EventHandler ReturnToList;

        #region Constructors

        // Khởi tạo màn hình chi tiết khách hàng
        public CustomerDetailView()
        {
            InitializeComponent();
        }

        #endregion

        #region Public methods

        // Nạp dữ liệu chi tiết cho một khách hàng
        public void LoadCustomer(Customer customer)
        {
            if (customer == null) return;

            txtMaKH.Text = customer.MaKH;
            txtTenKH.Text = customer.TenKH;
            txtSDT.Text = customer.SDT;
            txtHangThanhVien.Text = customer.HangThanhVien;
            txtDoanhThu.Text = string.Format("{0:N0} VNĐ", customer.DoanhThu);

            txtDiaChi.Text = "123 Nguyễn Văn Linh, Quận 7, TP.HCM";
            txtEmail.Text = customer.MaKH.ToLower() + "@email.com";

            txtDiemTichLuy.Text = "1,500 điểm";

            LoadPurchaseHistory();
        }

        #endregion

        #region Private methods

        // Tải dữ liệu lịch sử mua hàng mẫu cho khách hàng hiện tại
        private void LoadPurchaseHistory()
        {
            var histories = new List<PurchaseHistory>
            {
                new PurchaseHistory { STT = 1, MaHD = "HD001", TongTien = "500,000 VNĐ", NgayMuaHang = "01/11/2024", GhiChu = "Mua sách văn học" },
                new PurchaseHistory { STT = 2, MaHD = "HD002", TongTien = "750,000 VNĐ", NgayMuaHang = "10/11/2024", GhiChu = "Mua sách giáo khoa" },
                new PurchaseHistory { STT = 3, MaHD = "HD003", TongTien = "1,200,000 VNĐ", NgayMuaHang = "20/11/2024", GhiChu = "Mua sách tham khảo" },
                new PurchaseHistory { STT = 4, MaHD = "HD004", TongTien = "300,000 VNĐ", NgayMuaHang = "25/11/2024", GhiChu = "Mua văn phòng phẩm" }
            };

            dgHistories.ItemsSource = histories;
        }

        // Xử lý khi người dùng nhấn nút quay lại danh sách khách hàng
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            ReturnToList?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }

    #region Models

    // Model lịch sử mua hàng dùng hiển thị trong DataGrid
    public class PurchaseHistory
    {
        public int STT { get; set; }
        public string MaHD { get; set; }
        public string TongTien { get; set; }
        public string NgayMuaHang { get; set; }
        public string GhiChu { get; set; }
    }

    #endregion
}
