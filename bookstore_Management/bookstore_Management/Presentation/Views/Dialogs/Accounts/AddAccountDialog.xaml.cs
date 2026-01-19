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

namespace bookstore_Management.Presentation.Views.Dialogs.Accounts
{
    /// <summary>
    /// Interaction logic for AddBookDialog.xaml
    /// </summary>
    public partial class AddAccountDialog : Window
    {
        public AddAccountDialog()
        {
            InitializeComponent();


            // Generate temporary book ID
            tbAccountID.Text = "Tự động tạo khi lưu";
        }


        public void LoadStaffId(List<string> staffIds)
        {
            cbStaffId.ItemsSource = staffIds;
            if (staffIds != null && staffIds.Count > 0)
            {
                cbStaffId.SelectedIndex = 0;
            }
        }
        #region Properties for Data Binding

        public string AccountID
        {
            get { return tbAccountID.Text; }
            set { tbAccountID.Text = value; }
        }

        public string Username
        {
            get { return tbUsername.Text; }
            set { tbUsername.Text = value; }
        }

        public string Password
        {
            get { return tbPassword.Text; }
            set { tbPassword.Text = value; }
        }

        public string SelectedStaffID
        {
            get => cbStaffId.SelectedItem as string;
        }
       
        //public UserRole Role
        //{
        //    get { return (UserRole)cbRole.SelectedItem; }
        //    set { cbRole.SelectedItem = value; }
        //}

        #endregion

        #region Event Handlers

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate all required fields
            if (!ValidateForm())
                return;

            // Lấy mã nhân viên từ ComboBox
            var staffID = cbStaffId.SelectedItem as string; 

            if (string.IsNullOrWhiteSpace(staffID))
            {
                ShowValidationError("Vui lòng chọn mã nhân viên!");
                cbStaffId.Focus();
                return;
            }

            // Nếu có DTO để trả ra ngoài/đẩy vào service, map ở đây:
            // var dto = new CreateBookRequestDto {
            //     BookName = tbBookName.Text.Trim(),
            //     Author = tbAuthor.Text.Trim(),
            //     Category = (BookCategory)cbCategory.SelectedItem,
            //     PublisherName = publisherName,
            //     ImportPrice = ImportPrice,
            //     SalePrice = SalePrice
            // };

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

        #endregion

        #region Validation Methods

        private bool ValidateForm()
        {
            // Check StaffID
            if (cbStaffId.SelectedItem == null)
            {
                ShowValidationError("Vui lòng chọn mã nhân viên!");
                cbStaffId.Focus();
                return false;
            }

            // Check Username
            if (string.IsNullOrWhiteSpace(tbUsername.Text))
            {
                ShowValidationError("Vui lòng nhập tên đăng nhập!");
                tbUsername.Focus();
                return false;
            }

            if (tbUsername.Text.Length < 2)
            {
                ShowValidationError("Tên đăng nhập phải có ít nhất 2 ký tự!");
                tbUsername.Focus();
                return false;
            }

            // Check Password
            if (string.IsNullOrWhiteSpace(tbPassword.Text))
            {
                ShowValidationError("Vui lòng nhập mật khẩu!");
                tbPassword.Focus();
                return false;
            }

            if (tbPassword.Text.Length < 6)
            {
                ShowValidationError("Mật khẩu phải có ít nhất 6 ký tự!");
                tbPassword.Focus();
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
            return !string.IsNullOrWhiteSpace(tbUsername.Text) ||
                   !string.IsNullOrWhiteSpace(tbPassword.Text);
        }

        #endregion


    }
}
