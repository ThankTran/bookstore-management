using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Models;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using Moq;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
namespace bookstore_Management.Presentation.ViewModels
{
    internal class BookViewModel : BaseViewModel
    {
        #region các khai báo
        //lấy service
        private readonly IBookService _bookService;      

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
        private string _searchKeywork;
        public string SearchKeywork
        {
            get => _searchKeywork;
            set
            {
                _searchKeywork = value;
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

        public BookViewModel(IBookService bookService)
        {
            //_bookService = bookService ?? new BookService();
            var context = new BookstoreDbContext();   
            
            _bookService = new BookService(
            new BookRepository(context),
            new PublisherRepository(context),
            new ImportBillDetailRepository(context)
            );
            

            Books = new ObservableCollection<Book>();

            LoadBooksFromDatabase();

            #region AddCommand
            AddBookCommand = new RelayCommand<object>((p) =>
            {
                var dialog = new Views.Dialogs.Books.AddBookDialog();
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
                if (book == null)
                {
                    MessageBox.Show("Vui lòng chọn sách để chỉnh sửa");
                    return;
                }

                //đưa dữ liệu cũ lên dialog
                dialog.BookID = book.BookId;
                dialog.BookName = book.Name;
                dialog.Author = book.Author;
                dialog.Category = book.Category;
                //dialog.SalePrice = book.SalePrice;
                //dialog.Publisher = book.Publisher;
                // Giả sử Dialog có property SelectedPublisherId hoặc bạn gán trực tiếp cho ComboBox
                //if (book.Publisher != null)
                //{
                //    dialog.SelectedPublisherId = book.Publisher.Id;
                //    // ComboBox trong dialog sẽ tự nhảy đến NXB tương ứng dựa trên ID này
                //}

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
                if (string.IsNullOrEmpty(SearchKeywork))
                {
                    LoadBooksFromDatabase();//k nhập gì thì hiện lại list
                    return;
                }

                var result = _bookService.SearchByName(SearchKeywork);
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

            //chưa làm xong
            #region PrintCommand 
            PrintCommand = new RelayCommand<object>((p) =>
            {

            });
            #endregion

            #region ExportCommand
            ExportCommand = new RelayCommand<object>((p) =>
            {

            });
            #endregion

        }
    }
}

