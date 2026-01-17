using bookstore_Management.DTOs.ImportBill.Requests;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace bookstore_Management.Presentation.Views.Dialogs.Invoices
{
    /// <summary>
    /// Interaction logic for CreateImportBill.xaml
    /// </summary>
    public partial class CreateImportBill : Window
    {
        #region Properties

        private ObservableCollection<ImportBookItem> _bookItems;

        public string PublisherId => cbPublisher.SelectedValue?.ToString();
        public string Notes => tbNotes.Text.Trim();
        public string CreatedBy => tbCreatedBy.Text.Trim();

        public CreateImportBillRequestDto GetImportBillData()
        {
            return new CreateImportBillRequestDto
            {
                PublisherId = PublisherId,
                Notes = Notes,
                CreatedBy = CreatedBy,
                ImportBillDetails = _bookItems.Select(item => new ImportBillDetailCreateRequestDto
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    ImportPrice = item.ImportPrice
                }).ToList()
            };
        }

        #endregion

        #region Constructor

        public CreateImportBill()
        {
            InitializeComponent();

            _bookItems = new ObservableCollection<ImportBookItem>();
            icBooks.ItemsSource = _bookItems;

            // Subscribe to collection changed event to update total
            _bookItems.CollectionChanged += (s, e) => UpdateTotalAmount();
        }

        #endregion

        #region Window Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Set default values
            tbCreatedDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            // Load publishers - This should be done via ViewModel/Service
            // For now, leaving it empty - you should inject ISupplierService
            // LoadPublishers();

            // Focus on first input
            cbPublisher.Focus();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        #endregion

        #region Button Events

        private void BtnAddBook_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PublisherId))
            {
                MessageBox.Show(
                    "Vui lòng chọn nhà xuất bản trước khi thêm sách!",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                cbPublisher.Focus();
                return;
            }

            // Open SelectBookDialog - auto-load books from database
            var dialog = new SelectBookDialog(PublisherId)
            {
                Owner = this
            };

            if (dialog.ShowDialog() == true)
            {
                var bookItem = dialog.SelectedBookItem;
                if (bookItem != null)
                {
                    AddBookToList(bookItem);
                }
            }
        }

        private void BtnRemoveBook_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button?.Tag as ImportBookItem;

            if (item != null)
            {
                var result = MessageBox.Show(
                    $"Bạn có chắc muốn xóa sách '{item.BookName}' khỏi danh sách?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _bookItems.Remove(item);
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
            {
                return;
            }

            var confirmResult = MessageBox.Show(
                $"Bạn có chắc muốn tạo phiếu nhập với tổng tiền {CalculateTotalAmount():N0} ₫?",
                "Xác nhận tạo phiếu nhập",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmResult == MessageBoxResult.Yes)
            {
                DialogResult = true;
                Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_bookItems.Count > 0)
            {
                var result = MessageBox.Show(
                    "Bạn có chắc muốn hủy? Tất cả dữ liệu đã nhập sẽ bị mất.",
                    "Xác nhận hủy",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            DialogResult = false;
            Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            BtnCancel_Click(sender, e);
        }

        #endregion

        #region ComboBox Events

        private void CbPublisher_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // When publisher changes, you might want to clear the book list
            // or filter available books
            if (_bookItems.Count > 0)
            {
                var result = MessageBox.Show(
                    "Thay đổi nhà xuất bản sẽ xóa danh sách sách đã chọn. Bạn có chắc không?",
                    "Xác nhận",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _bookItems.Clear();
                }
                else
                {
                    // Revert selection
                    e.Handled = true;
                }
            }
        }

        #endregion

        #region TextBox Events

        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BookItem_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTotalAmount();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Add book to import list
        /// </summary>
        public void AddBookToList(ImportBookItem bookItem)
        {
            // Check if book already exists
            var existing = _bookItems.FirstOrDefault(b => b.BookId == bookItem.BookId);
            if (existing != null)
            {
                MessageBox.Show(
                    $"Sách '{bookItem.BookName}' đã có trong danh sách!",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            _bookItems.Add(bookItem);
        }

        /// <summary>
        /// Calculate total amount
        /// </summary>
        private decimal CalculateTotalAmount()
        {
            return _bookItems.Sum(item => item.Subtotal);
        }

        /// <summary>
        /// Update total amount display
        /// </summary>
        private void UpdateTotalAmount()
        {
            var total = CalculateTotalAmount();
            tbTotalAmount.Text = $"{total:N0} ₫";
        }

        /// <summary>
        /// Validate form before saving
        /// </summary>
        private bool ValidateForm()
        {
            // Check Publisher
            if (string.IsNullOrEmpty(PublisherId))
            {
                ShowValidationError("Vui lòng chọn nhà xuất bản!");
                cbPublisher.Focus();
                return false;
            }

            // Check CreatedBy
            if (string.IsNullOrWhiteSpace(CreatedBy))
            {
                ShowValidationError("Vui lòng nhập người tạo phiếu!");
                tbCreatedBy.Focus();
                return false;
            }

            // Check if there are books
            if (_bookItems.Count == 0)
            {
                ShowValidationError("Vui lòng thêm ít nhất một sách vào danh sách!");
                return false;
            }

            // Validate each book item
            foreach (var item in _bookItems)
            {
                if (item.Quantity <= 0)
                {
                    ShowValidationError($"Số lượng sách '{item.BookName}' phải lớn hơn 0!");
                    return false;
                }

                if (item.ImportPrice <= 0)
                {
                    ShowValidationError($"Giá nhập sách '{item.BookName}' phải lớn hơn 0!");
                    return false;
                }
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

        #region Public Methods for External Use

        /// <summary>
        /// Load publishers into ComboBox (should be called from outside)
        /// </summary>
        public void LoadPublishers(System.Collections.IEnumerable publishers)
        {
            cbPublisher.ItemsSource = publishers;
        }

        /// <summary>
        /// Set default created by (e.g., current user)
        /// </summary>
        public void SetCreatedBy(string userId, string userName)
        {
            tbCreatedBy.Text = $"{userId} - {userName}";
        }

        #endregion
    }

    #region Helper Classes

    /// <summary>
    /// Class representing a book item in the import bill
    /// </summary>
    public class ImportBookItem : INotifyPropertyChanged
    {
        private int _quantity;
        private decimal _importPrice;

        public string BookId { get; set; }
        public string BookName { get; set; }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                    OnPropertyChanged(nameof(Subtotal));
                }
            }
        }

        public decimal ImportPrice
        {
            get => _importPrice;
            set
            {
                if (_importPrice != value)
                {
                    _importPrice = value;
                    OnPropertyChanged(nameof(ImportPrice));
                    OnPropertyChanged(nameof(Subtotal));
                }
            }
        }

        public decimal Subtotal => Quantity * ImportPrice;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    #endregion
}

