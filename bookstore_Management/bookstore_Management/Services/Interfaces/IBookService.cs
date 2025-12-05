
 using System.Collections.Generic;
 using bookstore_Management.Core.Enums;
 using bookstore_Management.Core.Results;
 using bookstore_Management.DTOs;
 using bookstore_Management.Models;

namespace bookstore_Management.Services.Interfaces
{
    internal interface IBookService
    {
        // CRUD cho Book
        Result<string> AddBook(BookDto dto);
        Result UpdateBook(string bookId, BookDto dto);
        Result DeleteBook(string bookId);
        Result<Book> GetBookById(string bookId);
        Result<IEnumerable<Book>> GetAllBooks();
        
        // Tìm kiếm & Lọc
        Result<IEnumerable<Book>> SearchByName(string keyword);
        Result<IEnumerable<Book>> GetByCategory(BookCategory category);
        Result<IEnumerable<Book>> GetBySupplier(string supplierId);
        
        // Quản lý tồn kho
        Result UpdateStock(string bookId, int quantity);
        Result CheckStock(string bookId, int requiredQuantity);
        Result<IEnumerable<Book>> GetLowStockBooks(int minStock = 5);
        
        // Quản lý giá
        Result UpdatePrice(string bookId, decimal newSalePrice);
        Result<decimal> CalculateProfit(string bookId);
    }
}
