using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Models;
using bookstore_Management.Services.Implementations;
using bookstore_Management.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs.Book.Responses;

namespace bookstore_Management.Presentation.ViewModels
{
    internal class BookViewModel : BaseViewModel
    {
        #region các khai báo
        //lấy service
        private readonly IBookService _bookService;      
        //dữ liệu để view binding
        private ObservableCollection<Book> _books;
        private ObservableCollection<Book> Books
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

        //command cho in / xuất excel
        public ICommand ExportCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        #endregion

        #region Load book from db
        private async Task LoadBooksFromDatabase()
        {
            var result = await _bookService.GetAllBooksAsync();
            if (!result.IsSuccess)
            {
                // Xử lý lỗi, để sau này làm thông báo lỗi sau
                MessageBox.Show("Lỗi khi tải dữ liệu sách: " + result.ErrorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (result.Data == null) return; // Tránh lỗi khi Data rỗng

            var books = result.Data.Select(MapDtoToBook);

            Books = new ObservableCollection<Book>(books);
        }
        #endregion

        #region constructor
        public BookViewModel()
        {
            //_bookService = bookService ?? new BookService();
            var context = new BookstoreDbContext();   
            var unitOfWork = new  UnitOfWork(context);

            _bookService = new BookService(unitOfWork);
            

            Books = new ObservableCollection<Book>();
            _ = LoadBooksFromDatabase();

            #region AddCommand
            AddBookCommand = new RelayCommand<object>(p => 
            Task.Run(async () =>
                {
                var dialog = new Views.Dialogs.Books.AddBookDialog();
                if (dialog.ShowDialog() == true)
                {
                    // Call service to add book to database
                    var newBookDto = new DTOs.Book.Requests.CreateBookRequestDto
                    {
                        Name = dialog.BookName,
                        Author = dialog.Author,
                        Category = dialog.Category,
                        SalePrice = dialog.SalePrice,
                        PublisherName = dialog.cbPublisher.SelectedItem as string,
                    };
                    var result = await _bookService.CreateBookAsync(newBookDto);
                    if (!result.IsSuccess)
                    {
                        MessageBox.Show("Lỗi khi thêm sách: " + result.ErrorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    // Reload books from database
                    _ = LoadBooksFromDatabase();
                }
            }));
            #endregion
            #region RemoveCommand
            RemoveBookCommand = new RelayCommand<object>((p) =>
            Task.Run(async () =>
                {
                var book = p as Book;
                if (book == null)
                {
                    MessageBox.Show("Vui lòng chọn sách để xóa");
                    return;
                }

                var confirmed = Views.Dialogs.Share.Delete.ShowForBook(
                    bookName: book.Name,
                    bookId: book.BookId
                );

                if (!confirmed) return;

                var result = await _bookService.DeleteBookAsync(book.BookId);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("Lỗi khi xóa sách: " + result.ErrorMessage,
                                    "Lỗi",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return;
                }
                _ = LoadBooksFromDatabase();
            }));
            #endregion
            #region EditCommand
            EditBookCommand = new RelayCommand<object>((p) => 
        Task.Run(async () =>
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

                    var result = await _bookService.UpdateBookAsync(book.BookId, updateDto);
                    if (!result.IsSuccess)
                    {
                        MessageBox.Show("Lỗi khi cập nhật / chỉnh sửa sách");
                        return;
                    }

                    LoadBooksFromDatabase();
                }
            }));
            #endregion
            #region SearchCommand
            SearchBookCommand = new RelayCommand<object>((p) =>
            Task.Run( async () =>
                {
                if (string.IsNullOrEmpty(SearchKeyword))
                {
                    LoadBooksFromDatabase();//k nhập gì thì hiện lại list
                    return;
                }

                var result = await _bookService.SearchByNameAsync(SearchKeyword);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("Lỗi khi tìm sách");
                    return;
                }
                Books.Clear();
                foreach (var b in result.Data)
                {
                    Books.Add(MapDtoToBook(b));
                }
            }));
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
        #endregion
        
        #region helper class
        private Book MapDtoToBook(BookDetailResponseDto dto)
        {
            return new Book
            {
                BookId = dto.BookId,
                Name = dto.Name,
                Author = dto.Author,
                Publisher = new Publisher
                {
                    Name = dto.PublisherName
                },
                Category = dto.Category,
                SalePrice = dto.SalePrice
            };
        }

        #endregion
    }
}

