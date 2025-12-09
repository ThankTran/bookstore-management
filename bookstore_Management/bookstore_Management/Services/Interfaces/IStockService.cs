using System.Collections.Generic;
using bookstore_Management.Core.Results;
using bookstore_Management.Models;
namespace bookstore_Management.Services.Interfaces
{
    public interface IStockService
    {
        // CRUD
        Result<Stock> GetStockByBookId(string bookId);
        Result<IEnumerable<Stock>> GetAllStocks();
        
        // Cập nhật số lượng
        Result AddStockQuantity(string bookId, int quantityToAdd);
        Result SubtractStockQuantity(string bookId, int quantityToSubtract);
        Result SetStockQuantity(string bookId, int newQuantity);

        // Kiểm tra
        Result<bool> CheckAvailableStock(string bookId, int requiredQuantity);
        Result<IEnumerable<Stock>> GetLowStockBooks(int minStock = 5);
        Result<IEnumerable<Stock>> GetOutOfStockBooks();
    }
}