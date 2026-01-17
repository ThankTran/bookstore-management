using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Models;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using Moq;
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
        #region Fields

        private List<Book> _allBooks;
        private List<Book> _filteredBooks;
        private Book _selectedBook;
        private string _publisherId;
        private readonly IBookService _bookService;

        #endregion

        #region Properties

        public ImportBookItem SelectedBookItem { get; private set; }

        #endregion

        #region Constructor

        public SelectBookDialog(string publisherId)
        {
            InitializeComponent();

            _publisherId = publisherId;
            _allBooks = new List<Book>();
            _filteredBooks = new List<Book>();

            // Initialize service
            var context = new BookstoreDbContext();
            var bookRepo = new BookRepository(context);
            var supplierRepo = new PublisherRepository(context);
            var importBillDetailRepo = new ImportBillDetailRepository(context);

            _bookService = new BookService(bookRepo, supplierRepo, importBillDetailRepo);
        }

        #endregion

        #region Window Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBooksFromDatabase();
            tbSearch.Focus();
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

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }

            // Create ImportBookItem
            SelectedBookItem = new ImportBookItem
            {
                BookId = _selectedBook.BookId,
                BookName = _selectedBook.Name,
                Quantity = int.Parse(tbQuantity.Text),
                ImportPrice = decimal.Parse(tbImportPrice.Text)
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

        #endregion

        #region Search Events

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterBooks(tbSearch.Text);
        }

        #endregion

        #region Book Item Events

        private void BookItem_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            var book = border?.Tag as Book;

            if (book != null)
            {
                SelectBook(book);
            }
        }

        private void BookItem_MouseEnter(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F0F0"));
            }
        }

        private void BookItem_MouseLeave(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                border.Background = Brushes.White;
            }
        }

        #endregion

        #region TextBox Events

        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Load books from database using IBookService
        /// </summary>
        private void LoadBooksFromDatabase()
        {
            try
            {
                // Get all books
                var result = _bookService.GetAllBooks();

                if (!result.IsSuccess)
                {
                    MessageBox.Show(
                        $"Lỗi khi tải danh sách sách: {result.ErrorMessage}",
                        "Lỗi",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                // Convert DTOs to Book entities for display
                // Filter by publisher if specified
                _allBooks = result.Data
                    .Where(dto => string.IsNullOrEmpty(_publisherId) || dto.PublisherName == _publisherId)
                    .Select(dto => new Book
                    {
                        BookId = dto.BookId,
                        Name = dto.Name,
                        Author = dto.Author,
                        Category = dto.Category,
                        SalePrice = dto.SalePrice,
                        PublisherId = _publisherId
                    })
                    .ToList();

                _filteredBooks = new List<Book>(_allBooks);

                UpdateBooksList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi tải danh sách sách: {ex.Message}",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Load books from database/service
        /// </summary>
        public void LoadBooks(IEnumerable<Book> books)
        {
            _allBooks = books?.Where(b =>
                b.DeletedDate == null &&
                (string.IsNullOrEmpty(_publisherId) || b.PublisherId == _publisherId)
            ).ToList() ?? new List<Book>();

            _filteredBooks = new List<Book>(_allBooks);

            UpdateBooksList();
        }

        /// <summary>
        /// Filter books by search text
        /// </summary>
        private void FilterBooks(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                _filteredBooks = new List<Book>(_allBooks);
            }
            else
            {
                var search = searchText.ToLower().Trim();
                _filteredBooks = _allBooks.Where(b =>
                    b.BookId.ToLower().Contains(search) ||
                    b.Name.ToLower().Contains(search) ||
                    b.Author.ToLower().Contains(search)
                ).ToList();
            }

            UpdateBooksList();
        }

        /// <summary>
        /// Update books list display
        /// </summary>
        private void UpdateBooksList()
        {
            icBooks.ItemsSource = null;
            icBooks.ItemsSource = _filteredBooks;

            // Show empty state if no books
            pnlEmpty.Visibility = _filteredBooks.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Select a book
        /// </summary>
        private void SelectBook(Book book)
        {
            _selectedBook = book;
            tbSelectedBook.Text = $"{book.BookId} - {book.Name}";
            btnAdd.IsEnabled = true;

            // Set focus to quantity
            tbQuantity.Focus();
            tbQuantity.SelectAll();
        }

        /// <summary>
        /// Validate input before adding
        /// </summary>
        private bool ValidateInput()
        {
            // Check if book selected
            if (_selectedBook == null)
            {
                ShowValidationError("Vui lòng chọn sách!");
                return false;
            }

            // Check quantity
            if (string.IsNullOrWhiteSpace(tbQuantity.Text))
            {
                ShowValidationError("Vui lòng nhập số lượng!");
                tbQuantity.Focus();
                return false;
            }

            if (!int.TryParse(tbQuantity.Text, out int quantity) || quantity <= 0)
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

            // Check import price
            if (string.IsNullOrWhiteSpace(tbImportPrice.Text))
            {
                ShowValidationError("Vui lòng nhập giá nhập!");
                tbImportPrice.Focus();
                return false;
            }

            if (!decimal.TryParse(tbImportPrice.Text, out decimal price) || price <= 0)
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
            MessageBox.Show(
                message,
                "Lỗi nhập liệu",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        #endregion
    }

    // Note: ImportBookItem class is already defined in CreateImportBill.xaml.cs
    // If you want to reuse it, consider moving it to a separate file in Models/DTOs
}