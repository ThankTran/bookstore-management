using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using Moq;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using bookstore_Management.Presentation.Views.Dialogs.Books;

namespace bookstore_Management.Presentation.ViewModels
{
    internal class BookViewModel : BaseViewModel
    {
        #region các khai báo
        //lấy service
        private readonly IBookService _bookService;
        private readonly IPublisherRepository _publisherRepository;

        //dữ liệu để view binding
        private ObservableCollection<Book> _books;
        public ObservableCollection<Book> Books
        {
            get { return _books; }
            set
            {
                _books = value;
                OnPropertyChanged();
            }
        }

        //để lấy data từ list
        public Array BookCategories => Enum.GetValues(typeof(BookCategory));

        private BookCategory _category;
        public BookCategory Category
        {
            get => _category;
            set
            {
                _category = value;
                OnPropertyChanged();
            }
        }

        //sách đã chọn để xóa/sửa
        private Book _selectedBook;
        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                OnPropertyChanged();
            }
        }

        //keyword để tìm kiếm
        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged();
                SearchBookCommand.Execute(null);
            }
        }
        #endregion

        #region Khai báo command
        //khai báo command cho thao tác thêm, xóa, sửa sách
        public ICommand AddBookCommand { get; set; }
        public ICommand RemoveBookCommand { get; set; }
        public ICommand EditBookCommand { get; set; }

        //command cho thao tác tìm kiếm - load lại
        public ICommand SearchBookCommand { get; set; }
        public ICommand LoadData { get; set; }

        //command cho in / xuất excel
        public ICommand ExportCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        #endregion

        #region Load book from db
        private void LoadBooksFromDatabase()
        {
            var result = _bookService.GetAllBooks();

            if (!result.IsSuccess)
            {
                // Xử lý lỗi, để sau này làm thông báo lỗi sau
                MessageBox.Show("Lỗi khi tải dữ liệu sách: " + result.ErrorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (result.Data == null) return; // Tránh lỗi khi Data rỗng

            var books = result.Data.Select(dto => new Book
            {
                BookId = dto.BookId,
                Name = dto.Name,
                Author = dto.Author,
                Publisher = new Publisher
                {
                    Name = dto.PublisherName 
                },
                Category = dto.Category,
                SalePrice = dto.SalePrice,
                Stock = dto.StockQuantity,
            });

            Books = new ObservableCollection<Book>(books);
        }
        #endregion

        #region constructor
        public BookViewModel(IUnitOfWork unitOfWork)
        {
            //_bookService = bookService ?? new BookService();
            var context = new BookstoreDbContext();
            _publisherRepository = new PublisherRepository(context);
            _bookService = new BookService(unitOfWork);
            

            Books = new ObservableCollection<Book>();

            LoadBooksFromDatabase();

            #region AddCommand
            AddBookCommand = new RelayCommand<object>((p) =>
            {
                var dialog = new Views.Dialogs.Books.AddBookDialog();

                var publishers = _publisherRepository.GetAll();
                var publisherNames = publishers.Select(x => x.Name).ToList();
                dialog.LoadPublishers(publisherNames);

                if (dialog.ShowDialog() == true)
                {
                    // Call service to add book to database
                    var newBookDto = new DTOs.Book.Requests.CreateBookRequestDto
                    {
                        Id = dialog.BookID,
                        Name = dialog.BookName,
                        Author = dialog.Author,
                        Category = dialog.Category,
                        SalePrice = dialog.SalePrice,
                        PublisherName = dialog.cbPublisher.SelectedItem as string,
                    };
                    var result = _bookService.CreateBook(newBookDto);
                    if (!result.IsSuccess)
                    {
                        MessageBox.Show("Lỗi khi thêm sách: " + result.ErrorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    // Reload books from database
                    LoadBooksFromDatabase();
                }
            });
            #endregion
            #region RemoveCommand
            RemoveBookCommand = new RelayCommand<object>((p) =>
            {
                var book = p as Book;
                if (book == null)
                {
                    MessageBox.Show("Vui lòng chọn sách để xóa");
                    return;
                }

                bool confirmed = Views.Dialogs.Share.Delete.ShowForBook(
                    bookName: book.Name,
                    bookId: book.BookId
                );

                if (!confirmed) return;

                var result = _bookService.DeleteBook(book.BookId);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("Lỗi khi xóa sách: " + result.ErrorMessage,
                                    "Lỗi",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return;
                }
                LoadBooksFromDatabase();
            });
            #endregion
            #region EditCommand
            EditBookCommand = new RelayCommand<object>((p) =>
            {
                var dialog = new Views.Dialogs.Books.UpdateBook();
                var book = p as Book;

                var publishers = _publisherRepository.GetAll();
                var publisherNames = publishers.Select(x => x.Name).ToList();

                // Nạp danh sách vào trước
                dialog.LoadPublishers(publisherNames);

                if (book == null)
                {
                    MessageBox.Show("Vui lòng chọn sách để chỉnh sửa");
                    return;
                }
                decimal safeSalePrice = book.SalePrice ?? 0;
                //đưa dữ liệu cũ lên dialog
                dialog.BookID = book.BookId;
                dialog.BookName = book.Name;
                dialog.Author = book.Author;
                dialog.Category = book.Category;
                dialog.SalePrice = safeSalePrice;

                if (dialog.ShowDialog() == true)
                {
                    var updateDto = new DTOs.Book.Requests.UpdateBookRequestDto
                    {
                        Name = book.Name,
                        Author = book.Author,
                        Category = book.Category,
                        SalePrice = book.SalePrice,
                        PublisherName = book.Publisher?.Name
                    };

                    var result = _bookService.UpdateBook(book.BookId, updateDto);
                    if (!result.IsSuccess)
                    {
                        MessageBox.Show("Lỗi khi cập nhật / chỉnh sửa sách");
                        return;
                    }

                    LoadBooksFromDatabase();
                }
            });
            #endregion
            #region SearchCommand
            SearchBookCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrEmpty(SearchKeyword))
                {
                    LoadBooksFromDatabase();//k nhập gì thì hiện lại list
                    return;
                }

                var result = _bookService.SearchByName(SearchKeyword);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("Lỗi khi tìm sách");
                    return;
                }
                Books.Clear();
                foreach (var b in result.Data)
                {
                    Books.Add(new Book
                    {
                        BookId = b.BookId,
                        Name = b.Name,
                        Author = b.Author,
                        Category = b.Category,
                        SalePrice = b.SalePrice,
                        Publisher = new Publisher
                        {
                            Name = b.PublisherName
                        },
                    });
                }
            });
            #endregion
            #region LoadDataCommand
            LoadData = new RelayCommand<object>((p) =>
            {
                SearchKeyword = string.Empty;
                LoadBooksFromDatabase();
            });
            #endregion

            #region Print
            PrintCommand = new RelayCommand<object>((p) =>
            {
                // Lấy danh sách đang hiển thị (Ví dụ: _datalist hoặc FilteredList)
                var data = Books;

                // Truyền data vào khi tạo cửa sổ
                var dialog = new Views.Dialogs.Books.PrintBook(data);

                dialog.ShowDialog();
            });
            #endregion
            #region ExportCommand
            ExportCommand = new RelayCommand<object>((p) =>
            {
                 try
                    {
                        var data = Books.ToList();
                        var export = new ExportExcelBook(data);
                        export.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "EXPORT ERROR");
                    }
            });
            #endregion

        }
        #endregion
    }
}

