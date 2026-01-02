using System;
using System.Windows;

namespace bookstore_Management.Presentation.Views.Dialogs.Accounts
{
    public partial class AddAccountDialog : Window
    {
        public string MaNV => txtMaNV.Text.Trim();
        public string TenNV => txtTenNV.Text.Trim();
        public string TenDangNhap => txtTenDangNhap.Text.Trim();
        public string MatKhau => txtMatKhau.Password;
        public string Email => txtEmail.Text.Trim();
        public string SDT => txtSDT.Text.Trim();
        public string ChucVu => (cboChucVu.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();

        public AddAccountDialog()
        {
            InitializeComponent();
            txtMaNV.Text = "NV" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(TenNV))
            {
                MessageBox.Show("Vui lòng nhập tên nhân viên!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTenNV.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(TenDangNhap))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTenDangNhap.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(MatKhau))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtMatKhau.Focus();
                return;
            }

            if (MatKhau.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtMatKhau.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                MessageBox.Show("Vui lòng nhập email!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return;
            }

            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}