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

        // Trả về cho CreateImportBill
        public ImportBookItem SelectedBookItem { get; private set; }

        public SelectBookDialog(string publisherId)
        {
            InitializeComponent();

            _publisherId = publisherId;

            // Tự khởi tạo service (để phù hợp CreateImportBill hiện tại đang new dialog trực tiếp)
            var context = new BookstoreDbContext();
            var bookRepo = new BookRepository(context);
            var publisherRepo = new PublisherRepository(context);
            var importBillDetailRepo = new ImportBillDetailRepository(context);

            _bookService = new BookService(bookRepo, publisherRepo, importBillDetailRepo);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBooksFromDatabase();
            tbSearch.Focus();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

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

        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // chỉ cho nhập số
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

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

                // ✅ Filter theo PublisherId (đúng nghiệp vụ)
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

        private bool ValidateInput()
        {
            if (_selectedBook == null)
            {
                ShowValidationError("Vui lòng chọn sách!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(tbQuantity.Text))
            {
                ShowValidationError("Vui lòng nhập số lượng!");
                tbQuantity.Focus();
                return false;
            }

            if (!int.TryParse(tbQuantity.Text.Trim(), out int quantity) || quantity <= 0)
            {
                ShowValidationError("Số lượng phải là số nguyên dương!");
                tbQuantity.Focus();
                return false;
            }

            if (quantity > 10000)
            {
                ShowValidationError("Số lượng không được vượt quá 10,000!");
                tbQuantity.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(tbImportPrice.Text))
            {
                ShowValidationError("Vui lòng nhập giá nhập!");
                tbImportPrice.Focus();
                return false;
            }

            if (!decimal.TryParse(tbImportPrice.Text.Trim(), out decimal price) || price <= 0)
            {
                ShowValidationError("Giá nhập phải là số dương!");
                tbImportPrice.Focus();
                return false;
            }

            if (price > 1000000000)
            {
                ShowValidationError("Giá nhập không được vượt quá 1 tỷ đồng!");
                tbImportPrice.Focus();
                return false;
            }

            return true;
        }

        private void ShowValidationError(string message)
        {
            MessageBox.Show(message, "Lỗi nhập liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
