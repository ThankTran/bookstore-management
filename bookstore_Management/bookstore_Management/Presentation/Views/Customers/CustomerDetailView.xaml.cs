using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Models;
using bookstore_Management.Services.Implementations;
using Castle.Core.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
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
            #region code cũ
            if (customer == null) return;

            txtMaKH.Text = customer.CustomerId;
            txtTenKH.Text = customer.Name;
            txtSDT.Text = customer.Phone;
            //txtHangThanhVien.Text = customer.MemberLevel;
            //txtDoanhThu.Text = string.Format("{0:N0} VNĐ", customer.DoanhThu);
            //txtDiaChi.Text = customer.Address.ToString();
            //txtDiaChi.Text = "123 Nguyễn Văn Linh, Quận 7, TP.HCM";
            //txtEmail.Text = customer.CustomerId.ToLower() + "@email.com";

            txtDiemTichLuy.Text = customer.LoyaltyPoints.ToString();

            LoadPurchaseHistory(customer.CustomerId);
            #endregion
        }

        #endregion

        #region Private methods

        // Tải dữ liệu lịch sử mua hàng mẫu cho khách hàng hiện tại
        //private void LoadPurchaseHistory()
        //{
        //    #region code cũ
        //    //    var histories = new List<PurchaseHistory>
        //    //    {
        //    //        new PurchaseHistory { STT = 1, MaHD = "HD001", TongTien = "500,000 VNĐ", NgayMuaHang = "01/11/2024", GhiChu = "Mua sách văn học" },
        //    //        new PurchaseHistory { STT = 2, MaHD = "HD002", TongTien = "750,000 VNĐ", NgayMuaHang = "10/11/2024", GhiChu = "Mua sách giáo khoa" },
        //    //        new PurchaseHistory { STT = 3, MaHD = "HD003", TongTien = "1,200,000 VNĐ", NgayMuaHang = "20/11/2024", GhiChu = "Mua sách tham khảo" },
        //    //        new PurchaseHistory { STT = 4, MaHD = "HD004", TongTien = "300,000 VNĐ", NgayMuaHang = "25/11/2024", GhiChu = "Mua văn phòng phẩm" }
        //    //    };

        //    //    dgHistories.ItemsSource = histories;
        //    #endregion
        //}
        private void LoadPurchaseHistory(string customerId)
        {
            try
            {
                // 1. Mở kết nối Database
                using (var context = new BookstoreDbContext())
                {
                    // 2. Truy vấn: Tìm các hóa đơn của khách hàng này
                    // Giả sử bảng hóa đơn tên là 'Orders' và có cột 'CustomerId'
                    var orders = context.Orders
                                        .Where(x => x.CustomerId == customerId && x.DeletedDate == null) // Lọc theo khách + chưa xóa
                                        .OrderByDescending(x => x.CreatedDate) // Đơn mới nhất lên đầu
                                        .ToList();

                    // 3. Nếu không có đơn nào thì dừng
                    if (orders == null || orders.Count == 0)
                    {
                        dgHistories.ItemsSource = null; // Xóa dữ liệu cũ trên lưới (nếu có)
                        return;
                    }

                    // 4. Chuyển đổi từ dữ liệu DB (Entity) sang dữ liệu hiển thị (PurchaseHistory)
                    var historyList = orders.Select(o => new PurchaseHistory
                    {
                        MaHD = o.OrderId, // Lấy Mã HĐ từ DB

                        // Format ngày tháng (ví dụ: 16/01/2026)
                        NgayMuaHang = o.CreatedDate.ToString("dd/MM/yyyy"),

                        // Format tiền tệ (ví dụ: 500,000 VNĐ)
                        TongTien = string.Format("{0:N0} VNĐ", o.TotalPrice),

                        GhiChu = ""// Ghi chú (nếu có)
                    }).ToList();

                    // 5. Đẩy danh sách vào DataGrid (tên là dgHistories bên XAML)
                    dgHistories.ItemsSource = historyList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải lịch sử mua hàng: " + ex.Message);
            }
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
        public string MaHD { get; set; }
        public string TongTien { get; set; }
        public string NgayMuaHang { get; set; }
        public string GhiChu { get; set; }
    }

    #endregion
}
