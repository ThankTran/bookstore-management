using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace bookstore_Management.Presentation.Views.Dialogs.Customers
{
    /// <summary>
    /// Interaction logic for AddCustomer.xaml
    /// </summary>
    public partial class AddCustomer : Window
    {
        public AddCustomer()
        {
            InitializeComponent();
        }
        #region Properties for Data Binding
        public string CustomerName
        {
            get { return tbCustomerName.Text.Trim(); }
            set { tbCustomerName.Text = value; }
        }

        public string Phone
        {
            get { return tbPhoneNumber.Text.Trim(); }
            set { tbPhoneNumber.Text = value; }
        }
        public string Email
        {
            get { return tbEmail.Text.Trim(); }
            set { tbEmail.Text = value; }
        }
        public string Address
        {
            get { return tbAddress.Text.Trim(); }
            set { tbAddress.Text = value; }
        }
        #endregion
        
        #region Event Handlers
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CustomerName))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng.",
                                "Thông báo",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Phone))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại.",
                                "Thông báo",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (HasUserEnteredData())
            {
                var result = MessageBox.Show(
                    "Bạn có chắc muốn hủy? Dữ liệu đã nhập sẽ bị mất.",
                    "Xác nhận hủy",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }
            this.DialogResult = false;
            this.Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // Numeric input validation
        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Only allow numbers
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion

        #region Validation Methods

        private bool ValidateForm()
        {
            // Check Customer Name
            if (string.IsNullOrWhiteSpace(tbCustomerName.Text))
            {
                ShowValidationError("Vui lòng nhập tên khách hàng!");
                tbCustomerName.Focus();
                return false;
            }

            if (tbCustomerName.Text.Length < 2)
            {
                ShowValidationError("Tên khách hàng phải có ít nhất 2 ký tự!");
                tbCustomerName.Focus();
                return false;
            }

            if (!Regex.IsMatch(tbCustomerName.Text, @"^[a-zA-Z\s]+$"))
            {
                ShowValidationError("Tên khách hàng chỉ được chứa chữ cái và khoảng trắng!");
                tbCustomerName.Focus();
                return false;
            }

            // Check Email 
            if (!string.IsNullOrWhiteSpace(tbEmail.Text))
            {
                Regex emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailRegex.IsMatch(tbEmail.Text))
                {
                    ShowValidationError("Vui lòng nhập địa chỉ email hợp lệ!");
                    tbEmail.Focus();
                    return false;
                }
            }

            // Check Phone
            if (string.IsNullOrWhiteSpace(tbPhoneNumber.Text))
            {
                ShowValidationError("Vui lòng nhập số điện thoại!");
                tbPhoneNumber.Focus();
                return false;
            }

            Regex phoneRegex = new Regex(@"^(0|\+84)(3[2-9]|5[6|8|9]|7[0|6-9]|8[1-5|8|9]|9[0-4|6-9])[0-9]{7}$");
            if (!phoneRegex.IsMatch(tbPhoneNumber.Text))
            {
                ShowValidationError("Vui lòng nhập số điện thoại hợp lệ!");
                tbPhoneNumber.Focus();
                return false;
            }

            // Check Address
            if (string.IsNullOrWhiteSpace(tbAddress.Text))
            {
                ShowValidationError("Vui lòng nhập địa chỉ!");
                tbAddress.Focus();
                return false;
            }

            return true;
        }

        private void ShowValidationError(string message)
        {
            MessageBox.Show(
                message,
                "Lỗi nhập liệu",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        private bool HasUserEnteredData()
        {
            return !string.IsNullOrWhiteSpace(tbCustomerName.Text) ||
                   !string.IsNullOrWhiteSpace(tbPhoneNumber.Text) ||
                   !string.IsNullOrWhiteSpace(tbEmail.Text) ||
                   !string.IsNullOrWhiteSpace(tbAddress.Text);
        }

        #endregion


    }
}
