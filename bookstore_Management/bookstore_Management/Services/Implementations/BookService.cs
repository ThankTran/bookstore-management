using System;
using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Core.Enums;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories;
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

        internal BookService(IBookRepository bookRepository, ISupplierRepository supplierRepository)
        {
            _bookRepository = bookRepository;
            _supplierRepository = supplierRepository;
        }

        public Result<string> AddBook(BookDto dto)
        {
            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Result<string>.Fail("Tên sách không được trống");
                    
                if (dto.ImportPrice <= 0)
                    return Result<string>.Fail("Giá nhập phải > 0");
                    
                // Check supplier exists
                var supplier = _supplierRepository.GetById(dto.SupplierId);
                if (supplier == null)
                    return Result<string>.Fail("Nhà cung cấp không tồn tại");
                
                // Generate Book ID
                var bookId = GenerateBookId();
                
                // Calculate sale price (import price + 30%)
                var salePrice = dto.ImportPrice * 1.3m;
                
                // Create book
                var book = new Book
                {
                    BookId = bookId,
                    Name = dto.Name.Trim(),
                    SupplierId = dto.SupplierId,
                    Category = dto.Category,
                    ImportPrice = dto.ImportPrice,
                    SalePrice = salePrice
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

        public Result UpdateBook(string bookId, BookDto dto)
        {
            try
            {
                var book = _bookRepository.GetById(bookId);
                if (book == null)
                    return Result.Fail("Sách không tồn tại");
                
                book.Name = dto.Name.Trim();
                book.SupplierId = dto.SupplierId;
                book.Category = dto.Category;
                book.ImportPrice = dto.ImportPrice;
                book.SalePrice = dto.ImportPrice * 1.3m;
                
                _bookRepository.Update(book);
                _bookRepository.SaveChanges();
                
                return Result.Success("Cập nhật thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

         public Result DeleteBook(string bookId)
        {
            /*
            try
            {
                var book = _bookRepository.Get(bookId);
                if (book == null)
                    return Result.Fail("Sách không tồn tại");
                
                _bookRepository.(bookId);
                _bookRepository.SaveChanges();
                
                return Result.Success("Xóa sách thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }*/
            return Result.Fail($"Lỗi: hehe");
        } 

        public Result<Book> GetBookById(string bookId)
        {
            try
            {
                var book = _bookRepository.GetById(bookId);
                if (book == null)
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
                var books = _bookRepository.GetAll();
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
                var books = _bookRepository.SearchByName(keyword);
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
                var books = _bookRepository.GetByCategory(category);
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
                var books = _bookRepository.Find(b => b.SupplierId == supplierId);
                return Result<IEnumerable<Book>>.Success(books);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Book>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result UpdateStock(string bookId, int quantity)
        {
            try
            {
                var book = _bookRepository.GetById(bookId);
                if (book == null)
                    return Result.Fail("Sách không tồn tại");
                
                // logic
                return Result.Success("Cập nhật tồn kho thành công");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result CheckStock(string bookId, int requiredQuantity)
        {
            try
            {
                var book = _bookRepository.GetById(bookId);
                if (book == null)
                    return Result.Fail("Sách không tồn tại");
                
                // logic
                return Result.Success("Còn hàng");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<Book>> GetLowStockBooks(int minStock = 5)
        {
            try
            {
                // logic
                return Result<IEnumerable<Book>>.Success(new List<Book>());
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Book>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result UpdatePrice(string bookId, decimal newSalePrice)
        {
            try
            {
                var book = _bookRepository.GetById(bookId);
                if (book == null)
                    return Result.Fail("Sách không tồn tại");
                
                book.SalePrice = newSalePrice;
                _bookRepository.Update(book);
                _bookRepository.SaveChanges();
                
                return Result.Success("Cập nhật giá thành công");
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
                if (book == null)
                    return Result<decimal>.Fail("Sách không tồn tại");
                
                if (!book.SalePrice.HasValue || book.ImportPrice == 0)
                    return Result<decimal>.Fail("Không thể tính lợi nhuận");
                
                var profit = book.SalePrice.Value - book.ImportPrice;
                return Result<decimal>.Success(profit);
            }
            catch (Exception ex)
            {
                return Result<decimal>.Fail($"Lỗi: {ex.Message}");
            }
        }

        
        /// <summary>
        /// Hàm book id generate
        /// </summary>
        /// <returns></returns>
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
