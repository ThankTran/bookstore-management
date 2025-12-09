using System;
using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.DTOs;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;

namespace bookstore_Management.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IStockRepository _stockRepository;

        internal BookService(
            IBookRepository bookRepository, 
            ISupplierRepository supplierRepository,
            IStockRepository stockRepository)
        {
            _bookRepository = bookRepository;
            _supplierRepository = supplierRepository;
            _stockRepository = stockRepository;
        }

        // ==================================================================
        // ---------------------- THÊM DỮ LIỆU ------------------------------
        // ==================================================================
        public Result<string> AddBook(BookDto dto)
        {
            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result<string>.Fail("Tên sách không được trống");
                    
                if (dto.ImportPrice <= 0)
                    return Result<string>.Fail("Giá nhập phải > 0");
                    
                var supplier = _supplierRepository.GetById(dto.SupplierId);
                if (supplier == null)
                    return Result<string>.Fail("Nhà cung cấp không tồn tại");
                
                // Generate Book ID
                var bookId = GenerateBookId();
                
                // Auto set SalePrice nếu không có
                decimal? salePrice = dto.SalePrice.HasValue ? dto.SalePrice : (dto.ImportPrice * 1.3m);
                
                // Create book
                var book = new Book
                {
                    BookId = bookId,
                    Name = dto.Name.Trim(),
                    SupplierId = dto.SupplierId,
                    Category = dto.Category,
                    SalePrice = salePrice,
                    ImportPrice = dto.ImportPrice,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = null,
                    DeletedDate = null
                };
                
                _bookRepository.Add(book);
                _bookRepository.SaveChanges();

                // Tạo stock record
                var stock = new Stock
                {
                    BookId = bookId,
                    StockQuantity = 0
                };
                _stockRepository.Add(stock);
                _stockRepository.SaveChanges();
                
                return Result<string>.Success(bookId, "Thêm sách thành công");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- SỬA DỮ LIỆU ------------------------------
        // ==================================================================
        public Result UpdateBook(string bookId, BookDto dto)
        {
            try
            {
                var book = _bookRepository.GetById(bookId);
                if (book == null || book.DeletedDate != null)
                    return Result.Fail("Sách không tồn tại");
                
                var supplier = _supplierRepository.GetById(dto.SupplierId);
                if (supplier == null)
                    return Result.Fail("Nhà cung cấp không tồn tại");
                
                book.Name = dto.Name.Trim();
                book.SupplierId = dto.SupplierId;
                book.Category = dto.Category;
                book.ImportPrice = dto.ImportPrice;
                book.SalePrice = dto.SalePrice.HasValue ? dto.SalePrice : (dto.ImportPrice * 1.3m);
                book.UpdatedDate = DateTime.Now;
                
                _bookRepository.Update(book);
                _bookRepository.SaveChanges();
                
                return Result.Success("Cập nhật sách thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ---------------------- XÓA DỮ LIỆU -------------------------------
        // ==================================================================
        public Result DeleteBook(string bookId)
        {
            try
            {
                var book = _bookRepository.GetById(bookId);
                if (book == null || book.DeletedDate != null)
                    return Result.Fail("Sách không tồn tại");
                
                // Soft delete
                book.DeletedDate = DateTime.Now;
                _bookRepository.Update(book);
                _bookRepository.SaveChanges();
                
                return Result.Success("Xóa sách thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- LẤY DỮ LIỆU ------------------------------
        // ==================================================================
        public Result<Book> GetBookById(string bookId)
        {
            try
            {
                var book = _bookRepository.GetById(bookId);
                if (book == null || book.DeletedDate != null)
                    return Result<Book>.Fail("Sách không tồn tại");
                    
                return Result<Book>.Success(book);
            }
            catch (Exception ex)
            {
                return Result<Book>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Book>> GetAllBooks()
        {
            try
            {
                var books = _bookRepository.GetAll()
                    .Where(b => b.DeletedDate == null)
                    .ToList();
                return Result<IEnumerable<Book>>.Success(books);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Book>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Book>> SearchByName(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    return Result<IEnumerable<Book>>.Success(new List<Book>());

                var books = _bookRepository.SearchByName(keyword)
                    .Where(b => b.DeletedDate == null)
                    .ToList();
                return Result<IEnumerable<Book>>.Success(books);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Book>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Book>> GetByCategory(BookCategory category)
        {
            try
            {
                var books = _bookRepository.GetByCategory(category)
                    .Where(b => b.DeletedDate == null)
                    .ToList();
                return Result<IEnumerable<Book>>.Success(books);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Book>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Book>> GetBySupplier(string supplierId)
        {
            try
            {
                var books = _bookRepository.Find(b => 
                    b.SupplierId == supplierId && b.DeletedDate == null)
                    .ToList();
                return Result<IEnumerable<Book>>.Success(books);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Book>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Book>> GetLowStockBooks(int minStock = 5)
        {
            try
            {
                var stocks = _stockRepository.Find(s => 
                    s.StockQuantity > 0 && s.StockQuantity <= minStock);

                var bookIds = stocks.Select(s => s.BookId).ToList();
                var books = _bookRepository.Find(b => 
                    bookIds.Contains(b.BookId) && b.DeletedDate == null)
                    .ToList();

                return Result<IEnumerable<Book>>.Success(books);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Book>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Book>> GetOutOfStockBooks()
        {
            try
            {
                var outOfStocks = _stockRepository.Find(s => s.StockQuantity == 0);
                var bookIds = outOfStocks.Select(s => s.BookId).ToList();
                
                var books = _bookRepository.Find(b => 
                    bookIds.Contains(b.BookId) && b.DeletedDate == null)
                    .ToList();

                return Result<IEnumerable<Book>>.Success(books);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Book>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ---------------------- QUẢN LÝ GIÁ -------------------------------
        // ==================================================================
        public Result UpdateSalePrice(string bookId, decimal newSalePrice)
        {
            try
            {
                if (newSalePrice <= 0)
                    return Result.Fail("Giá bán phải > 0");

                var book = _bookRepository.GetById(bookId);
                if (book == null || book.DeletedDate != null)
                    return Result.Fail("Sách không tồn tại");
                
                book.SalePrice = newSalePrice;
                book.UpdatedDate = DateTime.Now;
                _bookRepository.Update(book);
                _bookRepository.SaveChanges();
                
                return Result.Success("Cập nhật giá bán thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<decimal> CalculateProfit(string bookId)
        {
            try
            {
                var book = _bookRepository.GetById(bookId);
                if (book == null || book.DeletedDate != null)
                    return Result<decimal>.Fail("Sách không tồn tại");
                
                if (!book.SalePrice.HasValue || book.ImportPrice == 0)
                    return Result<decimal>.Fail("Không thể tính lợi nhuận");
                
                var profit = book.SalePrice.Value - book.ImportPrice;
                var profitPercent = (profit / book.ImportPrice) * 100;
                
                return Result<decimal>.Success(profit, 
                    $"Lợi nhuận: {profit:N0} VND ({profitPercent:F2}%)");
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        // ==================================================================
        // ----------------------- HÀM HELPER --------------------------------
        // ==================================================================
        private string GenerateBookId()
        {
            var lastBook = _bookRepository.GetAll()
                .OrderByDescending(b => b.BookId)
                .FirstOrDefault();
                
            if (lastBook == null || !lastBook.BookId.StartsWith("S"))
                return "S00001";
                
            var lastNumber = int.Parse(lastBook.BookId.Substring(1));
            return $"S{(lastNumber + 1):D5}";
        }
    }
}