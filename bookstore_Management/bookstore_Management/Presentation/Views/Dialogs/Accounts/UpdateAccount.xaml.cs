using bookstore_Management.Core.Enums;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace bookstore_Management.Presentation.Views.Dialogs.Accounts
{
    public partial class UpdateAccount : Window
    {
        private bool _hasChanges = false;


        public UpdateAccount()
        {
            InitializeComponent();

            // Set default dates
            txtCreatedDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            txtLastModified.Text = "Chưa có";

            // Track changes
            cbAccountId.SelectionChanged += (s, e) => _hasChanges = true;
            tbPassword.TextChanged += (s, e) => _hasChanges = true;
        }
        public void LoadStaffIds(List<string> staffIds)
        {
            cbAccountId.ItemsSource = staffIds;
        }

        #region Properties for Data Binding
        public string Password
        {
            get { return tbPassword.Text; }
            set { tbPassword.Text = value; }
        }

        public string Account
        {
            get => cbAccountId.SelectedItem as string;
            set => cbAccountId.SelectedItem = value;
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
        public void LoadBAccountData(string accountid, string password,
                                 DateTime? createdDate = null, DateTime? lastModified = null)
        {
            cbAccountId.SelectedItem = accountid;
            Password = password;

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

            if (!ValidateForm())
                return;

            // Lấy account từ ComboBox (string)
            var publisherName = cbAccountId.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(publisherName))
            {
                ShowValidationError("Vui lòng chọn tài khoản!");
                cbAccountId.Focus();
                return;
            }


            // Confirm update
            var confirmResult = MessageBox.Show(
                $"Bạn có chắc muốn cập nhật thông tin tài khoản \"{cbAccountId.Text.Trim()}\"?",
                "Xác nhận cập nhật",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmResult == MessageBoxResult.No)
                return;

            // Update last modified time
            LastModifiedDate = DateTime.Now;

            #region Call Update Service (if neccessary)
            // Nếu có gọi service update thật sự, map dữ liệu ở đây (ví dụ):
            // var dto = new UpdateBookRequestDto
            // {
            //     BookId = tbBookID.Text.Trim(),
            //     BookName = tbBookName.Text.Trim(),
            //     Author = tbAuthor.Text.Trim(),
            //     PublisherName = publisherName,
            //     Category = (BookCategory)cbCategory.SelectedItem,
            //     ImportPrice = ImportPrice,
            //     SalePrice = SalePrice,
            //     LastModified = LastModifiedDate.Value
            // };
            // var result = _bookService.UpdateBook(dto);
            // if (!result.IsSuccess) { ShowValidationError(result.Message); return; }

            // Success - close dialog

            #endregion

            MessageBox.Show(
                "Cập nhật thông tin sách thành công!",
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

        #endregion

        #region Validation Methods

        private bool ValidateForm()
        {
            // Check Account ID
            if (cbAccountId.SelectedItem == null)
            {
                ShowValidationError("Vui lòng chọn tài khoản!");
                cbAccountId.Focus();
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

        #endregion
    }
}