using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using bookstore_Management.Views.Customers;
namespace bookstore_Management.Presentation.Views.Dialogs.Customers
{
    public partial class EditCustomer : Window
    {
        private Customer _customer;

        public EditCustomer()
        {
            InitializeComponent();
        }

        public EditCustomer(Customer customer) : this()
        {
            _customer = customer;
            LoadCustomerData();
        }

        private void LoadCustomerData()
        {
            if (_customer == null) return;

            txtCustomerCode.Text = _customer.MaKH;
            txtCustomerName.Text = _customer.TenKH;
            txtPhone.Text = _customer.SDT;

            txtEmail.Text = $"{_customer.MaKH.ToLower()}@email.com";
            txtAddress.Text = "123 Nguyễn Văn Linh, Quận 7, TP.HCM";
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCustomerName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPhone.Focus();
                return;
            }

            // Cập nhật trực tiếp lên _customer
            _customer.TenKH = txtCustomerName.Text.Trim();
            _customer.SDT = txtPhone.Text.Trim();

            MessageBox.Show("Cập nhật thông tin khách hàng thành công!",
                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

