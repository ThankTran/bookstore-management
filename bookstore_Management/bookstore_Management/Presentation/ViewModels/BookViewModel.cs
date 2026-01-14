using bookstore_Management.Core.Enums;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;
using bookstore_Management.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
namespace bookstore_Management.Presentation.ViewModels
{
    internal class BookViewModel : BaseViewModel
    {
        //lấy service
        private readonly IBookService _bookService = new BookService();

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

        //khai báo command cho thao tác thêm, xóa, sửa sách
        public ICommand AddBookCommand { get; set; }
        public ICommand RemoveBookCommand { get; set; }
        public ICommand EditBookCommand { get; set; }

        #region code lấy data mẫu cũ khi k dùng db
        //data mẫu cho 15 sách
        //public void LoadSampleData()
        //{
        //    Books = new ObservableCollection<Book>
        //    {
        //        new Book { bookSTT = 2, bookID = "VN002", author = "Nguyễn Nhật Ánh", name = "Mắt Biếc", category = "Tiểu thuyết", salePrice = 85000, importPrice = 60000, publisher = "NXB Trẻ" },
        //        new Book { bookSTT = 2, bookID = "VN002", author = "Nguyễn Nhật Ánh", name = "Cho tôi xin một vé đi tuổi thơ", category = "Tiểu thuyết", salePrice = 90000, importPrice = 65000, publisher = "NXB Trẻ" },
        //        new Book { bookSTT = 3, bookID = "VN003", author = "Nguyễn Minh Châu", name = "Chiếc thuyền ngoài xa", category = "Truyện ngắn", salePrice = 78000, importPrice = 55000, publisher = "NXB Văn học" },
        //        new Book { bookSTT = 4, bookID = "VN004", author = "Nam Cao", name = "Chí Phèo", category = "Truyện ngắn", salePrice = 70000, importPrice = 50000, publisher = "NXB Văn học" },
        //        new Book { bookSTT = 5, bookID = "VN005", author = "Ngô Tất Tố", name = "Tắt đèn", category = "Tiểu thuyết", salePrice = 75000, importPrice = 52000, publisher = "NXB Văn học" },
        //        new Book { bookSTT = 6, bookID = "VN006", author = "Nguyễn Huy Thiệp", name = "Tướng về hưu", category = "Truyện ngắn", salePrice = 80000, importPrice = 58000, publisher = "NXB Văn học" },
        //        new Book { bookSTT = 7, bookID = "VN007", author = "Xuân Quỳnh", name = "Thơ Xuân Quỳnh", category = "Thơ", salePrice = 65000, importPrice = 45000, publisher = "NXB Phụ nữ" },
        //        new Book { bookSTT = 8, bookID = "VN008", author = "Hồ Xuân Hương", name = "Thơ Hồ Xuân Hương", category = "Thơ", salePrice = 70000, importPrice = 48000, publisher = "NXB Văn học" },
        //        new Book { bookSTT = 9, bookID = "VN009", author = "Nguyễn Du", name = "Truyện Kiều", category = "Thơ", salePrice = 95000, importPrice = 70000, publisher = "NXB Văn học" },
        //        new Book { bookSTT = 10, bookID = "VN010", author = "Tô Hoài", name = "Dế Mèn phiêu lưu ký", category = "Thiếu nhi", salePrice = 85000, importPrice = 60000, publisher = "NXB Kim Đồng" },
        //        new Book { bookSTT = 11, bookID = "VN011", author = "Nguyễn Công Hoan", name = "Kép Tư Bền", category = "Truyện ngắn", salePrice = 72000, importPrice = 50000, publisher = "NXB Văn học" },
        //        new Book { bookSTT = 12, bookID = "VN012", author = "Thạch Lam", name = "Gió đầu mùa", category = "Truyện ngắn", salePrice = 76000, importPrice = 54000, publisher = "NXB Văn học" },
        //        new Book { bookSTT = 13, bookID = "VN013", author = "Nguyễn Khải", name = "Mùa lạc", category = "Tiểu thuyết", salePrice = 80000, importPrice = 58000, publisher = "NXB Văn học" },
        //        new Book { bookSTT = 14, bookID = "VN014", author = "Nguyễn Tuân", name = "Vang bóng một thời", category = "Truyện ngắn", salePrice = 82000, importPrice = 60000, publisher = "NXB Văn học" },
        //        new Book { bookSTT = 15, bookID = "EN015", author = "Victor Hugo", name = "Những người khốn khổ", category = "Kinh điển", salePrice = 140000, importPrice = 110000, publisher = "NXB Văn học nước ngoài" }
        //    };
        //}
        #endregion

        private void LoadBooksFromDatabase()
        {
            var result = _bookService.GetAllBooks();//Ở ĐÂY CÓ BUG 

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
                SupplierId = dto.SupplierName,
                Category = dto.Category,
                SalePrice = dto.SalePrice,
            });

            Books = new ObservableCollection<Book>(books);
        }
        public BookViewModel(IBookService bookService)
        {
            _bookService = bookService ?? new BookService();

            #region code cũ chưa xài database
            // LoadSampleData();

            //AddBookCommand = new RelayCommand<object>((p) =>
            //{
            //    var dialog = new Views.Dialogs.Books.InputBooksDialog();
            //    if(dialog.ShowDialog() == true)
            //    {
            //        // Logic to add a new book
            //        var newBook = new Book
            //        {
            //            bookSTT = Books.Count + 1,
            //            bookID = dialog.BookID,
            //            author = dialog.Author,
            //            name = dialog.BookName,
            //            category = "New Category",
            //            publisher = dialog.Publisher,
            //            salePrice = dialog.SalePrice,
            //            importPrice = dialog.ImportPrice
            //        };
            //        Books.Add(newBook);
            //    }               
            //});

            //RemoveBookCommand = new RelayCommand<Book>((book) =>
            //{
            //    var dialog = new Views.Dialogs.Books.ConfirmBooksDialog();
            //    if (dialog.ShowDialog() == true)
            //    {
            //        if (book != null && Books.Contains(book))
            //        {
            //            Books.Remove(book);
            //        }
            //    }
            //});

            //EditBookCommand = new RelayCommand<Book>((book) =>
            //{
            //    if (book != null)
            //    {
            //        book.name += " (Edited)";
            //        OnPropertyChanged(nameof(Books));
            //    }
            //});
            #endregion

            Books = new ObservableCollection<Book>();

            LoadBooksFromDatabase();

            AddBookCommand = new RelayCommand<object>((p) =>
            {
                var dialog = new Views.Dialogs.Books.AddBookDialog();
                if(dialog.ShowDialog() == true)
                {
                    // Call service to add book to database
                    var newBookDto = new DTOs.Book.Requests.CreateBookRequestDto
                    {
                        Id = dialog.BookID,
                        Name = dialog.BookName,
                        Author = dialog.Author,
                        Category = dialog.Category,
                        SalePrice = dialog.SalePrice,
                        SupplierId = dialog.Publisher
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
        }
    }

    //k cần xài class riêng nữa vì đã có model Book trong Data\Models
    //public class tempBook
    //{
    //    public string BookId { get; set; }
    //    public string Name { get; set; }
    //    public string Author { get; set; }
    //    public string SupplierId { get; set; }
    //    public BookCategory Category { get; set; }
    //    public decimal? SalePrice { get; set; }
    //}
}