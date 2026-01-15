using bookstore_Management.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace bookstore_Management.Presentation.Views.Dialogs.Staffs
{
    /// <summary>
    /// Interaction logic for AddStaff.xaml
    /// </summary>
    public partial class AddStaff : Window
    {
        public AddStaff()
        {
            InitializeComponent();

            // Set default values
            cbUserRole.SelectedIndex = 0;

            // Generate temporary staff ID
            tbStaffID.Text = "Tự động tạo khi lưu";
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

        #endregion

        #region Event Handlers

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate all required fields
            if (!ValidateForm())
            {
                return;
            }

            // Additional business logic validation
            

            // Success - close dialog
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Confirm if user has entered data
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
            this.Close();
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

        private bool HasUserEnteredData()
        {
            return !string.IsNullOrWhiteSpace(tbStaffName.Text) ||
                   !string.IsNullOrWhiteSpace(tbPhone.Text) ||
                   !string.IsNullOrWhiteSpace(tbCIC.Text);
        }

        #endregion


    }
}
