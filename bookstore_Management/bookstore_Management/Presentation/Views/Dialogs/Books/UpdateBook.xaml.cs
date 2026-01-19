using bookstore_Management.Core.Enums;
using System;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace bookstore_Management.Presentation.Views.Dialogs.Books
{
    public partial class UpdateBook : Window
    {
        private bool _hasChanges = false;


        public UpdateBook()
        {
            InitializeComponent();

            // Set default dates
            txtCreatedDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            txtLastModified.Text = "Chưa có";

            // Track changes
            tbBookName.TextChanged += (s, e) => _hasChanges = true;
            tbAuthor.TextChanged += (s, e) => _hasChanges = true;
            cbPublisher.SelectionChanged += (s, e) => _hasChanges = true;
            tbSalePrice.TextChanged += (s, e) => _hasChanges = true;
            cbCategory.SelectionChanged += (s, e) => _hasChanges = true;
        }
        public void LoadPublishers(System.Collections.IEnumerable publishers)
        {
            cbPublisher.ItemsSource = publishers;
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
            get { return cbCategory.SelectedItem != null ? (BookCategory)cbCategory.SelectedItem : BookCategory.Novel; }
            set { cbCategory.SelectedItem = value; }
        }

        public string Publisher
        {
            get => cbPublisher.SelectedItem as string;
            set => cbPublisher.SelectedItem = value; 
        }


        public decimal SalePrice
        {
            get { return decimal.TryParse(tbSalePrice.Text, out var val) ? val : 0; }
            set { tbSalePrice.Text = value.ToString("N0"); }
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
        public void LoadBookData(string bookId, string name, string author, string publisher,
                                 BookCategory category, decimal importPrice, decimal salePrice,
                                 DateTime? createdDate = null, DateTime? lastModified = null)
        {
            BookID = bookId;
            BookName = name;
            Author = author;
            cbPublisher.SelectedItem = publisher; 
            Category = category;
            SalePrice = salePrice;

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

            // Lấy publisher từ ComboBox (string)
            var publisherName = cbPublisher.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(publisherName))
            {
                ShowValidationError("Vui lòng chọn nhà xuất bản!");
                cbPublisher.Focus();
                return;
            }


            // Confirm update
            var confirmResult = MessageBox.Show(
                $"Bạn có chắc muốn cập nhật thông tin sách \"{tbBookName.Text.Trim()}\"?",
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
            if (cbPublisher.SelectedItem == null)
            {
                ShowValidationError("Vui lòng chọn nhà xuất bản!");
                cbPublisher.Focus();
                return false;
            }

            // Check Category
            if (cbCategory.SelectedItem == null)
            {
                ShowValidationError("Vui lòng chọn thể loại sách!");
                cbCategory.Focus();
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

        #endregion
    }
}