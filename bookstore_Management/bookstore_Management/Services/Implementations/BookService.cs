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
        private readonly IStockRepository _stockRepository;
        private readonly ISupplierRepository _supplierRepository;

        internal BookService(
            IBookRepository bookRepository,
            IStockRepository stockRepository,
            ISupplierRepository supplierRepository)
        {
            _bookRepository = bookRepository;
            _stockRepository = stockRepository;
            _supplierRepository = supplierRepository;
        }

        // ==================================================================
        // ---------------------- THÊM DỮ LIỆU ------------------------------
        // ==================================================================
        public Result<string> CreateBook(BookCreateDto dto)
        {
            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result<string>.Fail("Tên sách không được trống");
                if (string.IsNullOrWhiteSpace(dto.Author))
                    return Result<string>.Fail("Tác giả không được trống");

                if (!string.IsNullOrWhiteSpace(dto.SupplierId))
                {
                    var sup = _supplierRepository.GetById(dto.SupplierId);
                    if (sup == null || sup.DeletedDate != null)
                        return Result<string>.Fail("Nhà cung cấp không tồn tại");
                }

                // Generate Book ID
                var bookId = string.IsNullOrWhiteSpace(dto.Id) ? GenerateBookId() : dto.Id.Trim();
                if (_bookRepository.Exists(b => b.BookId == bookId))
                    return Result<string>.Fail("Mã sách đã tồn tại");

                // Auto set SalePrice nếu không có
                decimal? salePrice = dto.SalePrice;
                
                // Create book
                var book = new Book
                {
                    BookId = bookId,
                    Name = dto.Name.Trim(),
                    Author = dto.Author?.Trim(),
                    SupplierId = string.IsNullOrWhiteSpace(dto.SupplierId) ? null : dto.SupplierId.Trim(),
                    Category = dto.Category,
                    SalePrice = salePrice,
                    CreatedDate = DateTime.Now
                };
                
                _bookRepository.Add(book);
                _bookRepository.SaveChanges();

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
        public Result UpdateBook(string bookId, BookUpdateDto dto)
        {
            try
            {
                var book = _bookRepository.GetById(bookId);
                if (book == null || book.DeletedDate != null)
                    return Result.Fail("Sách không tồn tại");

                if (!string.IsNullOrWhiteSpace(dto.Name))
                    book.Name = dto.Name.Trim();
                if (!string.IsNullOrWhiteSpace(dto.Author))
                    book.Author = dto.Author.Trim();
                if (dto.Category.HasValue)
                    book.Category = dto.Category.Value;
                if (dto.SalePrice.HasValue)
                    book.SalePrice = dto.SalePrice.Value;
                if (!string.IsNullOrWhiteSpace(dto.SupplierId))
                {
                    var sup = _supplierRepository.GetById(dto.SupplierId);
                    if (sup == null || sup.DeletedDate != null)
                        return Result.Fail("Nhà cung cấp không tồn tại");
                    book.SupplierId = dto.SupplierId.Trim();
                }
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

        public Result<IEnumerable<Book>> GetByAuthor(string author)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(author))
                    return Result<IEnumerable<Book>>.Success(new List<Book>());

                var books = _bookRepository.GetByAuthor(author)
                    .Where(b => b.DeletedDate == null)
                    .ToList();
                return Result<IEnumerable<Book>>.Success(books);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Book>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Book>> GetByPriceRange(decimal? minPrice, decimal? maxPrice)
        {
            try
            {
                var books = _bookRepository.GetByPriceRange(minPrice, maxPrice)
                    .Where(b => b.DeletedDate == null)
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
                var stocks = _stockRepository.GetLowStock(minStock);
                var bookIds = stocks.Select(s => s.BookId).Distinct().ToList();
                var books = _bookRepository.Find(b => bookIds.Contains(b.BookId) && b.DeletedDate == null).ToList();
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
                var bookIds = outOfStocks.Select(s => s.BookId).Distinct().ToList();
                var books = _bookRepository.Find(b => bookIds.Contains(b.BookId) && b.DeletedDate == null).ToList();
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
                if (!book.SalePrice.HasValue || !book.ImportPrice.HasValue)
                    return Result<decimal>.Fail("Chưa hỗ trợ tính lợi nhuận (không có giá bán hoặc giá nhập)");
                  var profit = book.SalePrice.Value - book.ImportPrice.Value;
                return Result<decimal>.Success(profit, $"Lợi nhuận: {profit:N0} VND");
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