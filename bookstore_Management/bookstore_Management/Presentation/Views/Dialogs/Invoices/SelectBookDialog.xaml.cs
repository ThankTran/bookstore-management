using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.DTOs.Book.Responses;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace bookstore_Management.Presentation.Views.Dialogs.Invoices
{
    public partial class SelectBookDialog : Window
    {
        private readonly string _publisherId;
        private readonly IBookService _bookService;

        private List<BookDetailResponseDto> _allBooks = new List<BookDetailResponseDto>();
        private List<BookDetailResponseDto> _filteredBooks = new List<BookDetailResponseDto>();
        private BookDetailResponseDto _selectedBook;

        public ImportBookItem SelectedBookItem { get; private set; }

        public SelectBookDialog(string publisherId)
        {
            InitializeComponent();

            _publisherId = publisherId;

            var context = new BookstoreDbContext();
            var bookRepo = new BookRepository(context);
            var publisherRepo = new PublisherRepository(context);
            var importBillDetailRepo = new ImportBillDetailRepository(context);

            _bookService = new BookService(bookRepo, publisherRepo, importBillDetailRepo);
        }

        #region Window Dragging
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBooksFromDatabase();
            tbSearch.Focus();
        }


        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }
        #endregion

        #region Window Control Buttons
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            SelectedBookItem = new ImportBookItem
            {
                BookId = _selectedBook.BookId,
                BookName = _selectedBook.Name,
                Quantity = int.Parse(tbQuantity.Text.Trim()),
                ImportPrice = decimal.Parse(tbImportPrice.Text.Trim())
            };

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterBooks(tbSearch.Text);
        }
        #endregion

        #region Book Item Events

        private void BookItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is BookDetailResponseDto book)
            {
                SelectBook(book);
            }
        }

        private void BookItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F0F0"));
            }
        }

        private void BookItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = Brushes.White;
            }
        }

        #endregion

        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // chỉ cho nhập số
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        #region Loading Books
        private void LoadBooksFromDatabase()
        {
            try
            {
                var result = _bookService.GetAllBooks();

                if (!result.IsSuccess || result.Data == null)
                {
                    MessageBox.Show(result.ErrorMessage ?? "Không tải được danh sách sách!",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _allBooks = result.Data
                    .Where(dto => dto.PublisherId == _publisherId)
                    .ToList();

                _filteredBooks = new List<BookDetailResponseDto>(_allBooks);
                UpdateBooksList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách sách: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Filtering and Selection
        private void FilterBooks(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                _filteredBooks = new List<BookDetailResponseDto>(_allBooks);
            }
            else
            {
                var search = searchText.Trim().ToLower();
                _filteredBooks = _allBooks.Where(b =>
                        (b.BookId ?? "").ToLower().Contains(search) ||
                        (b.Name ?? "").ToLower().Contains(search) ||
                        (b.Author ?? "").ToLower().Contains(search)
                    )
                    .ToList();
            }

            UpdateBooksList();
        }

        private void UpdateBooksList()
        {
            icBooks.ItemsSource = null;
            icBooks.ItemsSource = _filteredBooks;

            pnlEmpty.Visibility = _filteredBooks.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SelectBook(BookDetailResponseDto book)
        {
            _selectedBook = book;
            tbSelectedBook.Text = $"{book.BookId} - {book.Name}";
            btnAdd.IsEnabled = true;

            tbQuantity.Focus();
            tbQuantity.SelectAll();
        }
        #endregion 

        #region Validation
        private bool ValidateInput()
        {
            // Validate selected book
            if (_selectedBook == null)
            {
                ShowValidationError("Vui lòng chọn sách!");
                return false;
            }

            // Validate quantity field
            if (string.IsNullOrWhiteSpace(tbQuantity.Text))
            {
                ShowValidationError("Vui lòng nhập số lượng!");
                tbQuantity.Focus();
                tbQuantity.SelectAll();
                return false;
            }

            var qtyText = tbQuantity.Text.Trim().Replace(",", "");
            if (!int.TryParse(qtyText, out int quantity))
            {
                ShowValidationError("Số lượng phải là số nguyên hợp lệ!");
                tbQuantity.Focus();
                tbQuantity.SelectAll();
                return false;
            }

            if (quantity <= 0)
            {
                ShowValidationError("Số lượng phải lớn hơn 0!");
                tbQuantity.Focus();
                tbQuantity.SelectAll();
                return false;
            }

            if (quantity > 10000)
            {
                ShowValidationError("Số lượng không được vượt quá 10,000!\nNếu cần nhập số lượng lớn hơn, vui lòng tạo nhiều phiếu nhập.");
                tbQuantity.Focus();
                tbQuantity.SelectAll();
                return false;
            }

            // Validate import price field
            if (string.IsNullOrWhiteSpace(tbImportPrice.Text))
            {
                ShowValidationError("Vui lòng nhập giá nhập!");
                tbImportPrice.Focus();
                tbImportPrice.SelectAll();
                return false;
            }

            var priceText = tbImportPrice.Text.Trim().Replace(",", "");
            if (!decimal.TryParse(priceText, out decimal price))
            {
                ShowValidationError("Giá nhập phải là số hợp lệ!");
                tbImportPrice.Focus();
                tbImportPrice.SelectAll();
                return false;
            }

            if (price <= 0)
            {
                ShowValidationError("Giá nhập phải lớn hơn 0!");
                tbImportPrice.Focus();
                tbImportPrice.SelectAll();
                return false;
            }

            if (price < 1000)
            {
                var confirm = MessageBox.Show(
                    $"Giá nhập ({price:N0} ₫) có vẻ thấp bất thường.\nBạn có chắc chắn muốn tiếp tục?",
                    "Xác nhận giá nhập",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (confirm != MessageBoxResult.Yes)
                {
                    tbImportPrice.Focus();
                    tbImportPrice.SelectAll();
                    return false;
                }
            }

            if (price > 1000000000)
            {
                ShowValidationError("Giá nhập không được vượt quá 1 tỷ đồng!\nVui lòng kiểm tra lại.");
                tbImportPrice.Focus();
                tbImportPrice.SelectAll();
                return false;
            }

            // Final confirmation for large orders
            var totalValue = quantity * price;
            if (totalValue > 100000000) // > 100 triệu
            {
                var confirm = MessageBox.Show(
                    $"Tổng giá trị đơn hàng: {totalValue:N0} ₫\n" +
                    $"Số lượng: {quantity:N0}\n" +
                    $"Giá nhập: {price:N0} ₫/cuốn\n\n" +
                    "Bạn có chắc chắn muốn tiếp tục?",
                    "Xác nhận đơn hàng lớn",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (confirm != MessageBoxResult.Yes)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        private void ShowValidationError(string message)
        {
            MessageBox.Show(message, "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
