using bookstore_Management.DTOs.ImportBill.Requests;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace bookstore_Management.Presentation.Views.Dialogs.Invoices
{
    public partial class CreateImportBill : Window
    {
        private readonly ObservableCollection<ImportBookItem> _bookItems = new ObservableCollection<ImportBookItem>();
        private object _previousPublisher;

        public string PublisherId => cbPublisher.SelectedValue?.ToString();
        public string Notes => tbNotes.Text?.Trim();
        public string CreatedBy => tbCreatedBy.Text?.Trim();

        public CreateImportBill()
        {
            InitializeComponent();

            icBooks.ItemsSource = _bookItems;

            // Nếu add/remove item thì update tổng
            _bookItems.CollectionChanged += (_, __) => UpdateTotalAmount();
        }

        #region Public API (lấy dữ liệu / nạp dữ liệu)

        public CreateImportBillRequestDto GetImportBillData()
        {
            return new CreateImportBillRequestDto
            {
                PublisherId = PublisherId,
                Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes,
                CreatedBy = CreatedBy,
                ImportBillDetails = _bookItems
                    .Where(x => !string.IsNullOrWhiteSpace(x.BookId))
                    .Select(x => new ImportBillDetailCreateRequestDto
                    {
                        BookId = x.BookId,
                        Quantity = x.Quantity,
                        ImportPrice = x.ImportPrice
                    })
                    .ToList()
            };
        }

        public void LoadPublishers(System.Collections.IEnumerable publishers)
        {
            cbPublisher.ItemsSource = publishers;
        }

        public void SetCreatedBy(string userId, string userName)
        {
            tbCreatedBy.Text = $"{userId} - {userName}";
        }

        #endregion

        #region Window Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbCreatedDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            _previousPublisher = cbPublisher.SelectedItem;
            cbPublisher.Focus();
            UpdateTotalAmount();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        #endregion

        #region Publisher selection

        private void CbPublisher_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_bookItems.Any())
            {
                _previousPublisher = cbPublisher.SelectedItem;
                return;
            }

            var result = MessageBox.Show(
                "Thay đổi nhà xuất bản sẽ xóa danh sách sách đã chọn. Bạn có chắc không?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                ClearBookItems();
                _previousPublisher = cbPublisher.SelectedItem;
            }
            else
            {
                cbPublisher.SelectionChanged -= CbPublisher_SelectionChanged;
                cbPublisher.SelectedItem = _previousPublisher;
                cbPublisher.SelectionChanged += CbPublisher_SelectionChanged;
            }
        }

        #endregion

        #region Buttons

        private void BtnAddBook_Click(object sender, RoutedEventArgs e)
        {
            var publisherName = (cbPublisher.SelectedItem as dynamic)?.Name?.ToString();
            // hoặc nếu SelectedItem là Publisher model thì:
            // var publisherName = (cbPublisher.SelectedItem as Publisher)?.Name;

            if (string.IsNullOrWhiteSpace(publisherName))
            {
                MessageBox.Show("Vui lòng chọn nhà xuất bản!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                cbPublisher.Focus();
                return;
            }

            var dialog = new SelectBookDialog(publisherName) { Owner = this };

            if (string.IsNullOrEmpty(PublisherId))
            {
                MessageBox.Show("Vui lòng chọn nhà xuất bản trước khi thêm sách!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                cbPublisher.Focus();
                return;
            }


            if (dialog.ShowDialog() == true && dialog.SelectedBookItem != null)
            {
                AddBookToList(dialog.SelectedBookItem);
            }
        }

        private void BtnRemoveBook_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button?.Tag as ImportBookItem;

            if (item == null) return;

            var result = MessageBox.Show(
                $"Bạn có chắc muốn xóa sách '{item.BookName}' khỏi danh sách?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            DetachItemEvents(item);
            _bookItems.Remove(item);
            UpdateTotalAmount();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            var total = CalculateTotalAmount();
            var confirmResult = MessageBox.Show(
                $"Bạn có chắc muốn tạo phiếu nhập với tổng tiền {total:N0} ₫?",
                "Xác nhận tạo phiếu nhập",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmResult != MessageBoxResult.Yes) return;

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_bookItems.Any())
            {
                var result = MessageBox.Show(
                    "Bạn có chắc muốn hủy? Tất cả dữ liệu đã nhập sẽ bị mất.",
                    "Xác nhận hủy",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No) return;
            }

            DialogResult = false;
            Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            BtnCancel_Click(sender, e);
        }

        #endregion

        #region Numeric input handlers (PreviewTextInput / KeyDown / Pasting)

        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // chỉ cho nhập số
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void NumericOnly_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Cho phép các phím điều hướng, xóa...
            if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Tab ||
                e.Key == Key.Left || e.Key == Key.Right)
                return;

            // Chặn space
            if (e.Key == Key.Space) e.Handled = true;
        }

        private void NumericOnly_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(typeof(string)))
            {
                e.CancelCommand();
                return;
            }

            var text = (string)e.DataObject.GetData(typeof(string));
            text = (text ?? "").Replace(",", "").Trim();

            if (!Regex.IsMatch(text, @"^\d+$"))
                e.CancelCommand();
        }

        #endregion

        #region Core logic

        private void AddBookToList(ImportBookItem bookItem)
        {
            // Check duplicate
            if (_bookItems.Any(b => b.BookId == bookItem.BookId))
            {
                MessageBox.Show($"Sách '{bookItem.BookName}' đã có trong danh sách!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // ensure defaults
            if (bookItem.Quantity <= 0) bookItem.Quantity = 1;
            if (bookItem.ImportPrice < 0) bookItem.ImportPrice = 0;

            AttachItemEvents(bookItem);

            _bookItems.Add(bookItem);
            UpdateTotalAmount();
        }

        private void AttachItemEvents(ImportBookItem item)
        {
            item.PropertyChanged += Item_PropertyChanged;
        }

        private void DetachItemEvents(ImportBookItem item)
        {
            item.PropertyChanged -= Item_PropertyChanged;
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ImportBookItem.Quantity) ||
                e.PropertyName == nameof(ImportBookItem.ImportPrice) ||
                e.PropertyName == nameof(ImportBookItem.Subtotal))
            {
                UpdateTotalAmount();
            }
        }

        private void ClearBookItems()
        {
            foreach (var item in _bookItems.ToList())
            {
                DetachItemEvents(item);
            }
            _bookItems.Clear();
            UpdateTotalAmount();
        }

        private decimal CalculateTotalAmount()
        {
            return _bookItems.Sum(item => item.Subtotal);
        }

        private void UpdateTotalAmount()
        {
            tbTotalAmount.Text = $"{CalculateTotalAmount():N0} ₫";
        }

        #region Validation
        private bool ValidateForm()
        {
            // Validate Publisher
            if (string.IsNullOrEmpty(PublisherId))
            {
                ShowValidationError("Vui lòng chọn nhà xuất bản!");
                cbPublisher.Focus();
                return false;
            }

            // Validate CreatedBy
            if (string.IsNullOrWhiteSpace(CreatedBy))
            {
                ShowValidationError("Vui lòng nhập người tạo phiếu!");
                tbCreatedBy.Focus();
                return false;
            }

            if (CreatedBy.Length < 3)
            {
                ShowValidationError("Tên người tạo phải có ít nhất 3 ký tự!");
                tbCreatedBy.Focus();
                return false;
            }

            // Validate book items
            if (!_bookItems.Any())
            {
                ShowValidationError("Vui lòng thêm ít nhất một sách vào danh sách!");
                return false;
            }

            // Check duplicates
            var duplicates = _bookItems
                .GroupBy(x => x.BookId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Any())
            {
                ShowValidationError($"Có sách bị trùng lặp trong danh sách: {string.Join(", ", duplicates)}\nVui lòng kiểm tra lại!");
                return false;
            }

            // Validate each item
            foreach (var item in _bookItems)
            {
                if (string.IsNullOrWhiteSpace(item.BookId))
                {
                    ShowValidationError("Có sách không hợp lệ (thiếu mã sách). Vui lòng chọn lại!");
                    return false;
                }

                if (item.Quantity <= 0)
                {
                    ShowValidationError($"Số lượng sách '{item.BookName}' phải lớn hơn 0!");
                    return false;
                }

                if (item.Quantity > 10000)
                {
                    ShowValidationError($"Số lượng sách '{item.BookName}' không được vượt quá 10,000!");
                    return false;
                }

                if (item.ImportPrice <= 0)
                {
                    ShowValidationError($"Giá nhập sách '{item.BookName}' phải lớn hơn 0!");
                    return false;
                }

                if (item.ImportPrice > 1_000_000_000m)
                {
                    ShowValidationError($"Giá nhập sách '{item.BookName}' không hợp lệ (quá lớn)!");
                    return false;
                }

                // Warn for suspiciously low prices
                if (item.ImportPrice < 1000)
                {
                    var confirm = MessageBox.Show(
                        $"Sách '{item.BookName}' có giá nhập rất thấp ({item.ImportPrice:N0} ₫).\n" +
                        "Bạn có chắc chắn muốn tiếp tục?",
                        "Cảnh báo giá nhập",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning
                    );

                    if (confirm != MessageBoxResult.Yes)
                    {
                        return false;
                    }
                }
            }


            // Validate total amount
            var totalAmount = CalculateTotalAmount();

            if (totalAmount <= 0)
            {
                ShowValidationError("Tổng tiền phải lớn hơn 0!");
                return false;
            }

            if (totalAmount > 10_000_000_000m) // > 10 tỷ
            {
                ShowValidationError("Tổng tiền vượt quá giới hạn cho phép (10 tỷ)!\nVui lòng tách thành nhiều phiếu nhập.");
                return false;
            }

            // Warn for large orders
            if (totalAmount > 500_000_000m) // > 500 triệu
            {
                var confirm = MessageBox.Show(
                    $"Tổng giá trị phiếu nhập: {totalAmount:N0} ₫\n" +
                    $"Số loại sách: {_bookItems.Count}\n" +
                    $"Tổng số lượng: {_bookItems.Sum(x => x.Quantity):N0} cuốn\n\n" +
                    "Bạn có chắc chắn muốn tạo phiếu nhập này?",
                    "Xác nhận phiếu nhập lớn",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (confirm != MessageBoxResult.Yes)
                {
                    return false;
                }
            }

            // Validate Notes length
            if (!string.IsNullOrWhiteSpace(Notes) && Notes.Length > 500)
            {
                ShowValidationError("Ghi chú không được vượt quá 500 ký tự!");
                tbNotes.Focus();
                return false;
            }

            return true;
        }
        #endregion

        private void ShowValidationError(string message)
        {
            MessageBox.Show(message, "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        #endregion
    }

    #region ImportBookItem class
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
                if (_quantity == value) return;
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(Subtotal));
            }
        }

        public decimal ImportPrice
        {
            get => _importPrice;
            set
            {
                if (_importPrice == value) return;
                _importPrice = value;
                OnPropertyChanged(nameof(ImportPrice));
                OnPropertyChanged(nameof(Subtotal));
            }
        }

        public decimal Subtotal => Quantity * ImportPrice;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    #endregion
}
