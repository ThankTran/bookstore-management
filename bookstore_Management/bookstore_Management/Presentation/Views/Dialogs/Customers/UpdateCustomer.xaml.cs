using bookstore_Management.Core.Enums;
using System;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace bookstore_Management.Presentation.Views.Dialogs.Customers
{
    public partial class UpdateCustomer : Window
    {
        private bool _hasChanges = false;


        public UpdateCustomer()
        {
            InitializeComponent();

            // Set default dates
            txtCreatedDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            txtLastModified.Text = "Chưa có";

            // Track changes
            tbCustomerName.TextChanged += (s, e) => _hasChanges = true;
            tbPhone.TextChanged += (s, e) => _hasChanges = true;
            tbEmail.TextChanged += (s, e) => _hasChanges = true;
            tbAddress.TextChanged += (s, e) => _hasChanges = true;
        }

        #region Properties for Data Binding

        public string CustomerID
        {
            get { return tbCustomerID.Text; }
            set { tbCustomerID.Text = value; }
        }

        public string CustomerName
        {
            get { return tbCustomerName.Text; }
            set { tbCustomerName.Text = value; }
        }

        public string Phone
        {
            get { return tbPhone.Text; }
            set { tbPhone.Text = value; }
        }

        public string Email
        {
            get { return tbEmail.Text; }
            set { tbEmail.Text = value; }
        }

        public string Address
        {
            get { return tbAddress.Text; }
            set { tbAddress.Text = value; }
        }

        public DateTime CreatedDate
        {
            get { return DateTime.TryParse(txtCreatedDate.Text, out var date) ? date : DateTime.Now; }
            set { txtCreatedDate.Text = value.ToString("dd/MM/yyyy HH:mm"); }
        }

        public DateTime? LastModifiedDate
        {
            get
            {
                if (txtLastModified.Text == "Chưa có") return null;
                return DateTime.TryParse(txtLastModified.Text, out var date) ? date : (DateTime?)null;
            }
            set
            {
                txtLastModified.Text = value.HasValue ? value.Value.ToString("dd/MM/yyyy HH:mm") : "Chưa có";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load book data into the dialog
        /// </summary>
        public void LoadBookData(string customerId, string name, string phone, string email, string address,
                                 DateTime? createdDate = null, DateTime? lastModified = null)
        {
            CustomerID = customerId;
            CustomerName = name;
            Phone = phone;
            Email = email;
            Address = address;

            if (createdDate.HasValue)
                CreatedDate = createdDate.Value;

            if (lastModified.HasValue)
                LastModifiedDate = lastModified.Value;

            // Reset change tracking after loading
            _hasChanges = false;
        }

        #endregion

        #region Event Handlers

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Check if any changes were made
            if (!_hasChanges)
            {
                var noChangeResult = MessageBox.Show(
                    "Bạn chưa thực hiện thay đổi nào. Bạn có muốn đóng cửa sổ?",
                    "Không có thay đổi",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                if (noChangeResult == MessageBoxResult.Yes)
                {
                    this.DialogResult = false;
                    this.Close();
                }
                return;
            }

            // Validate all required fields
            if (!ValidateForm())
            {
                return;
            }

            // Confirm update
            var confirmResult = MessageBox.Show(
                $"Bạn có chắc muốn cập nhật thông tin khách hàng \"{CustomerName}\"?",
                "Xác nhận cập nhật",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmResult == MessageBoxResult.No)
            {
                return;
            }

            // Update last modified time
            LastModifiedDate = DateTime.Now;

            // Success - close dialog
            MessageBox.Show(
                "Cập nhật thông tin khách hàng thành công!",
                "Thành công",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Confirm if user has made changes
            if (_hasChanges)
            {
                var result = MessageBox.Show(
                    "Bạn có thay đổi chưa được lưu. Bạn có chắc muốn hủy?",
                    "Xác nhận hủy",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

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
            BtnCancel_Click(sender, e);
        }

        // Numeric input validation - only allow numbers
        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
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

            // Check Phone Number
            if (string.IsNullOrWhiteSpace(tbPhone.Text))
            {
                ShowValidationError("Vui lòng nhập số điện thoại khách hàng!");
                tbPhone.Focus();
                return false;
            }

            Regex phoneRegex = new Regex(@"^(0|\+84)(3[2-9]|5[6|8|9]|7[0|6-9]|8[1-5|8|9]|9[0-4|6-9])[0-9]{7}$");
            if (!phoneRegex.IsMatch(tbPhone.Text))
            {
                ShowValidationError("Vui lòng nhập số điện thoại hợp lệ!");
                tbPhone.Focus();
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

            // Check Address
            if (string.IsNullOrWhiteSpace(tbAddress.Text))
            {
                ShowValidationError("Vui lòng nhập địa chỉ khách hàng!");
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

        #endregion
    }
}