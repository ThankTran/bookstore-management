using System;
using System.Collections.Generic;
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

namespace bookstore_Management.Presentation.Views.Dialogs.Share
{
    /// <summary>
    /// Dialog thông báo không có quyền xóa
    /// </summary>
    public partial class NDelete : Window
    {
        public NDelete()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor với thông tin tùy chỉnh
        /// </summary>
        /// <param name="userRole">Vai trò hiện tại</param>
        /// <param name="requiredRole">Vai trò yêu cầu</param>
        /// <param name="itemName">Tên đối tượng muốn xóa</param>
        /// <param name="reason">Lý do (optional)</param>
        /// <param name="suggestion">Gợi ý (optional)</param>
        public NDelete(string userRole, string requiredRole, string itemName,
            string reason = null, string suggestion = null) : this()
        {
            txtUserRole.Text = userRole;
            txtRequiredRole.Text = requiredRole;
            txtItemName.Text = itemName;

            // Set reason nếu có
            if (!string.IsNullOrEmpty(reason))
            {
                txtReason.Text = reason;
                pnlReason.Visibility = Visibility.Visible;
            }

            // Set suggestion nếu có
            if (!string.IsNullOrEmpty(suggestion))
            {
                txtSuggestion.Text = suggestion;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Focus vào nút OK
            BtnOk.Focus();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        #region Static Helper Methods

        /// <summary>
        /// Hiển thị dialog không có quyền xóa đơn giản
        /// </summary>
        public static void Show(string userRole, string requiredRole, string itemName, Window owner = null)
        {
            var dialog = new NDelete(userRole, requiredRole, itemName);
            if (owner != null) dialog.Owner = owner;
            dialog.ShowDialog();
        }

        /// <summary>
        /// Hiển thị dialog với lý do và gợi ý
        /// </summary>
        public static void Show(string userRole, string requiredRole, string itemName,
            string reason, string suggestion, Window owner = null)
        {
            var dialog = new NDelete(userRole, requiredRole, itemName, reason, suggestion);
            if (owner != null) dialog.Owner = owner;
            dialog.ShowDialog();
        }

        /// <summary>
        /// Không có quyền xóa sách
        /// </summary>
        public static void ShowForBook(string userRole, string bookName, Window owner = null)
        {
            string reason = "Thao tác xóa sách có thể ảnh hưởng đến toàn bộ hệ thống kho và báo cáo.";
            string suggestion = "Chỉ Quản lý hoặc Quản trị viên mới được phép xóa sách khỏi hệ thống. " +
                              "Vui lòng liên hệ với cấp quản lý để được hỗ trợ.";

            Show(userRole, "Quản lý/Admin", $"Sách '{bookName}'", reason, suggestion, owner);
        }

        /// <summary>
        /// Không có quyền xóa khách hàng
        /// </summary>
        public static void ShowForCustomer(string userRole, string customerName, Window owner = null)
        {
            string reason = "Xóa khách hàng có thể ảnh hưởng đến lịch sử giao dịch và báo cáo doanh thu.";
            string suggestion = "Chỉ Quản lý hoặc Quản trị viên mới được phép xóa thông tin khách hàng. " +
                              "Vui lòng liên hệ với cấp quản lý để được hỗ trợ.";

            Show(userRole, "Quản lý/Admin", $"Khách hàng '{customerName}'", reason, suggestion, owner);
        }

        /// <summary>
        /// Không có quyền xóa nhân viên
        /// </summary>
        public static void ShowForStaff(string userRole, string staffName, Window owner = null)
        {
            string reason = "Xóa nhân viên có thể ảnh hưởng đến phân quyền và lịch sử công việc.";
            string suggestion = "Chỉ Quản trị viên mới được phép xóa tài khoản nhân viên. " +
                              "Vui lòng liên hệ với Quản trị viên hệ thống.";

            Show(userRole, "Admin", $"Nhân viên '{staffName}'", reason, suggestion, owner);
        }

        /// <summary>
        /// Không có quyền xóa nhà xuất bản
        /// </summary>
        public static void ShowForPublisher(string userRole, string publisherName, Window owner = null)
        {
            string suggestion = "Chỉ Quản lý hoặc Quản trị viên mới được phép xóa nhà xuất bản. " +
                              "Vui lòng liên hệ với cấp quản lý để được hỗ trợ.";

            Show(userRole, "Quản lý/Admin", $"NXB '{publisherName}'", null, suggestion, owner);
        }

        /// <summary>
        /// Không có quyền xóa tài khoản
        /// </summary>
        public static void ShowForAccount(string userRole, string username, Window owner = null)
        {
            string reason = "Xóa tài khoản là thao tác nhạy cảm, yêu cầu quyền cao nhất.";
            string suggestion = "Chỉ Quản trị viên mới được phép xóa tài khoản người dùng. " +
                              "Vui lòng liên hệ với Quản trị viên hệ thống.";

            Show(userRole, "Admin", $"Tài khoản '{username}'", reason, suggestion, owner);
        }

        /// <summary>
        /// Không có quyền xóa hóa đơn
        /// </summary>
        public static void ShowForInvoice(string userRole, string invoiceId, Window owner = null)
        {
            string reason = "Hủy hóa đơn ảnh hưởng trực tiếp đến báo cáo tài chính và thuế.";
            string suggestion = "Chỉ Quản lý hoặc Kế toán mới được phép hủy hóa đơn. " +
                              "Vui lòng liên hệ với Phòng Kế toán hoặc cấp quản lý.";

            Show(userRole, "Quản lý/Kế toán", $"Hóa đơn '{invoiceId}'", reason, suggestion, owner);
        }

        /// <summary>
        /// Kiểm tra quyền và hiển thị dialog nếu không có quyền
        /// </summary>
        public static bool CheckPermission(bool hasPermission, string userRole,
            string requiredRole, string itemName, Window owner = null)
        {
            if (!hasPermission)
            {
                Show(userRole, requiredRole, itemName, owner);
                return false;
            }
            return true;
        }

        #endregion
    }
}
