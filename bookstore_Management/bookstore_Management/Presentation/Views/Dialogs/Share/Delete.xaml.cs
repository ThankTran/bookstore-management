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
    /// Interaction logic for Delete.xaml
    /// </summary>
    public partial class Delete : Window
    {
        public bool IsConfirmed { get; private set; }

        public Delete()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor với thông tin chi tiết
        /// </summary>
        /// <param name="itemType">Loại đối tượng (VD: "Sách", "Khách hàng")</param>
        /// <param name="itemName">Tên đối tượng</param>
        /// <param name="itemId">Mã đối tượng (optional)</param>
        /// <param name="warning">Cảnh báo tùy chỉnh (optional)</param>
        /// <param name="additionalInfo">Thông tin bổ sung (optional)</param>
        public Delete(string itemType, string itemName, string itemId = null,
            string warning = null, string additionalInfo = null) : this()
        {
            txtItemType.Text = itemType;
            txtItemName.Text = itemName;

            // Set Item ID nếu có
            if (!string.IsNullOrEmpty(itemId))
            {
                txtItemId.Text = itemId;
                pnlItemId.Visibility = Visibility.Visible;
            }

            // Set custom warning nếu có
            if (!string.IsNullOrEmpty(warning))
            {
                txtWarning.Text = warning;
            }

            // Set additional info nếu có
            if (!string.IsNullOrEmpty(additionalInfo))
            {
                txtAdditionalInfo.Text = additionalInfo;
                pnlAdditionalInfo.Visibility = Visibility.Visible;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Focus vào nút Cancel để tránh nhấn nhầm Delete
            BtnCancel.Focus();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = true;
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            DialogResult = false;
            Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            DialogResult = false;
            Close();
        }

        #region Static Helper Methods

        /// <summary>
        /// Hiển thị dialog xác nhận xóa đơn giản
        /// </summary>
        public static bool Show(string itemType, string itemName, Window owner = null)
        {
            var dialog = new Delete(itemType, itemName);
            if (owner != null) dialog.Owner = owner;

            dialog.ShowDialog();
            return dialog.IsConfirmed;
        }

        /// <summary>
        /// Hiển thị dialog xác nhận xóa với mã đối tượng
        /// </summary>
        public static bool Show(string itemType, string itemName, string itemId, Window owner = null)
        {
            var dialog = new Delete(itemType, itemName, itemId);
            if (owner != null) dialog.Owner = owner;

            dialog.ShowDialog();
            return dialog.IsConfirmed;
        }

        /// <summary>
        /// Hiển thị dialog xác nhận xóa đầy đủ
        /// </summary>
        public static bool Show(string itemType, string itemName, string itemId,
            string warning, string additionalInfo, Window owner = null)
        {
            var dialog = new Delete(itemType, itemName, itemId, warning, additionalInfo);
            if (owner != null) dialog.Owner = owner;

            dialog.ShowDialog();
            return dialog.IsConfirmed;
        }

        /// <summary>
        /// Xóa sách
        /// </summary>
        public static bool ShowForBook(string bookName, string bookId = null, Window owner = null)
        {
            string warning = "Sách sẽ bị xóa khỏi hệ thống và không thể khôi phục.";
            string additionalInfo = "Lưu ý: Nếu sách đã có trong đơn hàng, thông tin lịch sử vẫn được giữ lại.";

            return Show("Sách", bookName, bookId, warning, additionalInfo, owner);
        }

        /// <summary>
        /// Xóa khách hàng
        /// </summary>
        public static bool ShowForCustomer(string customerName, string customerId = null, Window owner = null)
        {
            string warning = "Khách hàng sẽ bị xóa khỏi hệ thống và không thể khôi phục.";
            string additionalInfo = "Lưu ý: Lịch sử giao dịch của khách hàng này sẽ được chuyển sang trạng thái 'Khách vãng lai'.";

            return Show("Khách hàng", customerName, customerId, warning, additionalInfo, owner);
        }

        /// <summary>
        /// Xóa nhân viên
        /// </summary>
        public static bool ShowForStaff(string staffName, string staffId = null, Window owner = null)
        {
            string warning = "Nhân viên sẽ bị xóa khỏi hệ thống và không thể đăng nhập.";
            string additionalInfo = "Lưu ý: Lịch sử làm việc và các giao dịch của nhân viên này sẽ được lưu trữ.";

            return Show("Nhân viên", staffName, staffId, warning, additionalInfo, owner);
        }

        /// <summary>
        /// Xóa nhà xuất bản
        /// </summary>
        public static bool ShowForPublisher(string publisherName, string publisherId = null, Window owner = null)
        {
            string warning = "Nhà xuất bản sẽ bị xóa khỏi hệ thống.";
            string additionalInfo = "Lưu ý: Các sách của nhà xuất bản này vẫn được giữ lại trong hệ thống.";

            return Show("Nhà xuất bản", publisherName, publisherId, warning, additionalInfo, owner);
        }

        /// <summary>
        /// Xóa tài khoản
        /// </summary>
        public static bool ShowForAccount(string username, string userId = null, Window owner = null)
        {
            string warning = "Tài khoản sẽ bị vô hiệu hóa và không thể đăng nhập.";
            string additionalInfo = "Lưu ý: Để đảm bảo tính toàn vẹn dữ liệu, tài khoản sẽ được chuyển sang trạng thái 'Đã xóa' thay vì xóa hoàn toàn.";

            return Show("Tài khoản", username, userId, warning, additionalInfo, owner);
        }

        /// <summary>
        /// Xóa hóa đơn
        /// </summary>
        public static bool ShowForInvoice(string invoiceId, Window owner = null)
        {
            string warning = "Hóa đơn sẽ bị hủy và không thể khôi phục.";
            string additionalInfo = "Lưu ý: Hủy hóa đơn sẽ ảnh hưởng đến báo cáo doanh thu. Vui lòng cân nhắc kỹ trước khi thực hiện.";

            return Show("Hóa đơn", invoiceId, invoiceId, warning, additionalInfo, owner);
        }

        #endregion
    }
}
