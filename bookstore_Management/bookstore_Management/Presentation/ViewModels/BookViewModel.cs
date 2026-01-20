using bookstore_Management.Core.Enums;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using bookstore_Management.DTOs.Book.Requests;
using CommunityToolkit.Mvvm.Input;

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
        private ObservableCollection<Book> Books
        {
            get => _books;
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
        private string SearchKeyword
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
        private ICommand SearchBookCommand { get; set; }
        public ICommand LoadData { get; set; }

        //command cho in / xuất excel
        public ICommand ExportCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        
        public IAsyncRelayCommand LoadBooksCommand  { get; }
        #endregion
        
        #region Load book from db
        public async Task LoadBooksFromDatabase()
        {
            var result =  await _bookService.GetAllBooksAsync();

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
        public BookViewModel(IBookService bookService, IPublisherRepository publisherRepo)
        {
            _bookService = bookService;
            _publisherRepository = publisherRepo;

            Books = new ObservableCollection<Book>();

            LoadBooksCommand = new AsyncRelayCommand(LoadBooksFromDatabase);
            AddBookCommand = new AsyncRelayCommand(AddBookAsync);
            RemoveBookCommand = new AsyncRelayCommand(RemoveBookAsync);
            EditBookCommand = new AsyncRelayCommand(EditBookAsync);
            SearchBookCommand = new AsyncRelayCommand(SearchBookAsync);
            LoadData = new AsyncRelayCommand(LoadBooksAsync);

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

            });
            #endregion

        }
        #endregion
        
        #region AddCommand
        private async Task AddBookAsync()
        {
            var dialog = new Views.Dialogs.Books.AddBookDialog();

            var publishers = await _publisherRepository.GetAllAsync();
            var publisherNames = publishers.Select(x => x.Name).ToList();

            dialog.LoadPublishers(publisherNames);

            if (dialog.ShowDialog() == true)
            {
                var newBookDto = new CreateBookRequestDto
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
                    MessageBox.Show("Lỗi khi thêm sách: " + result.ErrorMessage,
                        "Lỗi",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                await LoadBooksFromDatabase();
            }
        }
        #endregion

        #region RemoveCommand
        private async Task RemoveBookAsync()
        {
            if (SelectedBook  == null)
            {
                MessageBox.Show("Vui lòng chọn sách để xóa");
                return;
            }

            var confirmed = Views.Dialogs.Share.Delete.ShowForBook(
                bookName: SelectedBook .Name,
                bookId: SelectedBook .BookId
            );

            if (!confirmed) return;

            var result = await _bookService.DeleteBookAsync(SelectedBook .BookId);
            if (!result.IsSuccess)
            {
                MessageBox.Show("Lỗi khi xóa sách: " + result.ErrorMessage,
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            await LoadBooksFromDatabase();
        }
        #endregion
        
        #region EditCommand

        private async Task EditBookAsync()
        {
            var dialog = new Views.Dialogs.Books.UpdateBook();
            var book = SelectedBook;
            if (book == null)
            {
                MessageBox.Show("Vui lòng chọn sách để chỉnh sửa");
                return;
            }
            
            var publishers = await _publisherRepository.GetAllAsync();
            var publisherNames = publishers.Select(x => x.Name).ToList();

            // Nạp danh sách vào trước
            dialog.LoadPublishers(publisherNames);
            var safeSalePrice = book.SalePrice ?? 0;
            
            //đưa dữ liệu cũ lên dialog
            
            dialog.BookID = book.BookId;
            dialog.BookName = book.Name;
            dialog.Author = book.Author;
            dialog.Category = book.Category;
            dialog.SalePrice = safeSalePrice;
            dialog.Publisher = book.Publisher?.Name;

            if (dialog.ShowDialog() == true)
            {
                var updateDto = new UpdateBookRequestDto
                {
                    Name = dialog.Name,
                    Author = dialog.Author,
                    Category = dialog.Category,
                    SalePrice = dialog.SalePrice,
                    PublisherName = dialog.Publisher
                };

                var result = await _bookService.UpdateBookAsync(book.BookId, updateDto);
                if (!result.IsSuccess)
                {
                    MessageBox.Show("Lỗi khi cập nhật / chỉnh sửa sách");
                    return;
                }

                await LoadBooksFromDatabase();
            }
        }
        #endregion

        #region SearchBookCommand

        private async Task SearchBookAsync()
        {
            if (string.IsNullOrEmpty(SearchKeyword))
            {
                await LoadBooksFromDatabase();//k nhập gì thì hiện lại list
                return;
            }

            var result = await _bookService.SearchByNameAsync(SearchKeyword);
            
            
            if (!result.IsSuccess)
            {
                MessageBox.Show("Lỗi khi tìm sách");
                return;
            }
            if (result.Data == null)
                return;
            
            Books = new ObservableCollection<Book>(
                result.Data.Select(b => new Book
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
                })
            );
        }
        #endregion

        #region  loadDataCommand

        private async Task LoadBooksAsync()
        {
            SearchKeyword = string.Empty;
            await LoadBooksFromDatabase();
        }
        #endregion
    }
}

