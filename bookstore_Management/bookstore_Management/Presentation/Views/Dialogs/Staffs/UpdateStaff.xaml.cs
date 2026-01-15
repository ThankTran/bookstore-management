using bookstore_Management.Core.Enums;
using System;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace bookstore_Management.Presentation.Views.Dialogs.Staffs
{
    public partial class UpdateStaff : Window
    {
        private bool _hasChanges = false;


        public UpdateStaff()
        {
            InitializeComponent();

            // Set default dates
            txtCreatedDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            txtLastModified.Text = "Chưa có";

            // Track changes
            tbStaffName.TextChanged += (s, e) => _hasChanges = true;
            tbPhone.TextChanged += (s, e) => _hasChanges = true;
            tbCIC.TextChanged += (s, e) => _hasChanges = true;
            cbUserRole.SelectionChanged += (s, e) => _hasChanges = true;
        }

        #region Properties for Data Binding

        public string StaffID
        {
            get { return tbStaffID.Text; }
            set { tbStaffID.Text = value; }
        }

        public string StaffName
        {
            get { return tbStaffName.Text; }
            set { tbStaffName.Text = value; }
        }

        public UserRole Role
        {
            get { return (UserRole)cbUserRole.SelectedIndex; }
            set { cbUserRole.SelectedItem = value; }
        }

        public string PhoneNumber
        {
            get { return tbPhone.Text; }
            set { tbPhone.Text = value; }
        }

        public string CCCD
        {
            get { return tbCIC.Text; }
            set { tbCIC.Text = value; }
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
        /// Load staff data into the dialog
        /// </summary>
        public void LoadStaffData(string staffID, string name, string phone, string cccd, UserRole category, 
                                DateTime? createdDate = null, DateTime? lastModified = null)
        {
            StaffID = staffID;
            StaffName = name;
            Role = category;
            PhoneNumber = phone;
            CCCD = cccd;            

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
                $"Bạn có chắc muốn cập nhật thông tin nhân viên \"{StaffName}\"?",
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
                "Cập nhật thông tin nhân viên thành công!",
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


        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
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
            // Check Staff Name
            if (string.IsNullOrWhiteSpace(tbStaffName.Text))
            {
                ShowValidationError("Vui lòng nhập tên nhân viên!");
                tbStaffName.Focus();
                return false;
            }

            if (tbStaffName.Text.Trim().Length < 2)
            {
                ShowValidationError("Tên nhân viên phải có ít nhất 2 ký tự!");
                tbStaffName.Focus();
                return false;
            }

            if (!Regex.IsMatch(tbStaffName.Text, @"^[\p{L}\s]+$"))
            {
                ShowValidationError("Tên nhân viên chỉ được chứa chữ cái và khoảng trắng!");
                tbStaffName.Focus();
                return false;
            }

            // Check User Role
            if (cbUserRole.SelectedItem == null)
            {
                ShowValidationError("Vui lòng chọn vai trò cho nhân viên!");
                cbUserRole.Focus();
                return false;
            }

            // Check Phone
            if (string.IsNullOrWhiteSpace(tbPhone.Text))
            {
                ShowValidationError("Vui lòng nhập số điện thoại!");
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

            // Check CCCD
            if (string.IsNullOrWhiteSpace(tbCIC.Text))
            {
                ShowValidationError("Vui lòng nhập số căn cước công dân!");
                tbCIC.Focus();
                return false;
            }

            Regex cicRegex = new Regex(@"^\d{12}$");
            if (!cicRegex.IsMatch(tbCIC.Text))
            {
                ShowValidationError("Vui lòng nhập đúng định dạng CCCD");
                tbCIC.Focus();
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