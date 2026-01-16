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

namespace bookstore_Management.Presentation.Views.Dialogs.Books
{
    /// <summary>
    /// Interaction logic for AddBookDialog.xaml
    /// </summary>
    public partial class AddBookDialog : Window
    {
        public AddBookDialog()
        {
            InitializeComponent();

            // Set default values
            cbCategory.SelectedIndex = 0;

            // Generate temporary book ID
            tbBookID.Text = "Tự động tạo khi lưu";
        }

        #region Properties for Data Binding

        public string BookID
        {
            get { return tbBookID.Text; }
            set { tbBookID.Text = value; }
        }

        public string BookName
        {
            get { return tbBookName.Text; }
            set { tbBookName.Text = value; }
        }

        public string Author
        {
            get { return tbAuthor.Text; }
            set { tbAuthor.Text = value; }
        }

        public BookCategory Category
        {
            get { return (BookCategory)cbCategory.SelectedItem; }
            set { cbCategory.SelectedItem = value; }
        }

        public string Publisher
        {
            get { return tbPublisher.Text; }
            set { tbPublisher.Text = value; }
        }

        public decimal SalePrice => decimal.TryParse(tbSalePrice.Text, out var val) ? val : 0;

        public decimal ImportPrice => decimal.TryParse(tbImportPrice.Text, out var val) ? val : 0;

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
            if (SalePrice <= ImportPrice)
            {
                ShowValidationError("Giá bán phải lớn hơn giá nhập!");
                tbSalePrice.Focus();
                return;
            }

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
            // Check Book Name
            if (string.IsNullOrWhiteSpace(tbBookName.Text))
            {
                ShowValidationError("Vui lòng nhập tên sách!");
                tbBookName.Focus();
                return false;
            }

            if (tbBookName.Text.Length < 2)
            {
                ShowValidationError("Tên sách phải có ít nhất 2 ký tự!");
                tbBookName.Focus();
                return false;
            }

            // Check Author
            if (string.IsNullOrWhiteSpace(tbAuthor.Text))
            {
                ShowValidationError("Vui lòng nhập tên tác giả!");
                tbAuthor.Focus();
                return false;
            }

            if (!Regex.IsMatch(tbAuthor.Text, @"^[a-zA-Z\s]+$"))
            {
                ShowValidationError("Tên tác giả chỉ được chứa chữ cái và khoảng trắng!");
                tbAuthor.Focus();
                return false;
            }

            // Check Publisher
            if (string.IsNullOrWhiteSpace(tbPublisher.Text))
            {
                ShowValidationError("Vui lòng nhập nhà xuất bản!");
                tbPublisher.Focus();
                return false;
            }

            if (tbPublisher.Text.Length < 2)
            {
                ShowValidationError("Tên nhà xuất bản phải có ít nhất 2 ký tự!");
                tbPublisher.Focus();
                return false;
            }

            if (!Regex.IsMatch(tbPublisher.Text, @"^[a-zA-Z\s]+$"))
            {
                ShowValidationError("Tên nhà xuất bản chỉ được chứa chữ cái và khoảng trắng!");
                tbPublisher.Focus();
                return false;
            }

            // Check Category
            if (cbCategory.SelectedItem == null)
            {
                ShowValidationError("Vui lòng chọn thể loại sách!");
                cbCategory.Focus();
                return false;
            }

            // Check Import Price
            if (string.IsNullOrWhiteSpace(tbImportPrice.Text))
            {
                ShowValidationError("Vui lòng nhập giá nhập!");
                tbImportPrice.Focus();
                return false;
            }

            if (ImportPrice <= 0)
            {
                ShowValidationError("Giá nhập phải lớn hơn 0!");
                tbImportPrice.Focus();
                return false;
            }

            if (ImportPrice > 1000000000)
            {
                ShowValidationError("Giá nhập không hợp lệ (quá lớn)!");
                tbImportPrice.Focus();
                return false;
            }

            // Check Sale Price
            if (string.IsNullOrWhiteSpace(tbSalePrice.Text))
            {
                ShowValidationError("Vui lòng nhập giá bán!");
                tbSalePrice.Focus();
                return false;
            }

            if (SalePrice <= 0)
            {
                ShowValidationError("Giá bán phải lớn hơn 0!");
                tbSalePrice.Focus();
                return false;
            }

            if (SalePrice > 1000000000)
            {
                ShowValidationError("Giá bán không hợp lệ (quá lớn)!");
                tbSalePrice.Focus();
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
            return !string.IsNullOrWhiteSpace(tbBookName.Text) ||
                   !string.IsNullOrWhiteSpace(tbAuthor.Text) ||
                   !string.IsNullOrWhiteSpace(tbPublisher.Text) ||
                   !string.IsNullOrWhiteSpace(tbImportPrice.Text) ||
                   !string.IsNullOrWhiteSpace(tbSalePrice.Text);
        }

        #endregion


    }
}
