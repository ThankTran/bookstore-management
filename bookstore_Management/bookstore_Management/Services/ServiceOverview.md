# - Service Folder Overview -
###  1. Nội dung thư mục

Thư mục này bao gồm hai phần chính:
* Interfaces/ 
  * Chứa các hợp đồng (contract) mô tả service cần làm gì 
  * Không chứa logic xử lý
* Implementations/
  *   Chứa logic nghiệp vụ thực tế
  *   Thực thi các interface tương ứng

###  2. Các service chính

Hiện tại hệ thống có các service quan trọng:

    BookService
    CustomerService

Mỗi service sử dụng các repository để xử lý nghiệp vụ.

###  3. Cách sử dụng service trong UI/UX Layer

Để dùng service trong UI/ViewModel, cần truyền đúng đầy đủ **dependencies** vào **constructor**

Ví dụ constructor của BookService:

    internal BookService(
    IBookRepository bookRepository,
    IStockRepository stockRepository,
    ISupplierRepository supplierRepository,
    IImportBillDetailRepository importBillDetailRepository)
    {
    _bookRepository = bookRepository;
    _stockRepository = stockRepository;
    _supplierRepository = supplierRepository;
    _importBillDetailRepository = importBillDetailRepository;
    }

thì trong phần ViewModel ta cần phải khai báo như sau

    public class BookViewModel
    {
    private readonly IBookService _bookService;
    
        public BookViewModel()
        {
            using ( var context = new BookstoreDbContext())
            {
                _bookService = new BookService(
                new BookRepository(context),
                new StockRepository(context),
                new SupplierRepository(context),
                new ImportBillDetailRepository(context)
                );
            }
            
        }
    
        public void AddNewBook()
        {
            _bookService.AddBook(new Book { Title = "Example Book" });
        }
    }

###  Ghi chú quan trọng
* chỉ sử dụng các repository, implements để khai báo dependencies vào constructor
* sử dụng các Dtos để thêm, lấy dữ liệu từ database
* có thể đọc thêm trong file Tests để hiểu rõ hơn