using System.Collections.Generic;
using bookstore_Management.Core.Results;
using bookstore_Management.Models;
namespace bookstore_Management.Services.Interfaces
{
    public interface IStockService
    {
        Result<Stock> GetStock(string warehouseId, string bookId);
        Result<IEnumerable<Stock>> GetStocksByBook(string bookId);
        Result<IEnumerable<Stock>> GetStocksByWarehouse(string warehouseId);
        Result<IEnumerable<Stock>> GetAllStocks();
        Result AddStockQuantity(string warehouseId, string bookId, int quantityToAdd);
        Result SubtractStockQuantity(string warehouseId, string bookId, int quantityToSubtract);
        Result SetStockQuantity(string warehouseId, string bookId, int newQuantity);
        Result<bool> CheckAvailableStock(string bookId, int requiredQuantity);
        Result<IEnumerable<Stock>> GetLowStockBooks(int minStock = 5);
        Result<IEnumerable<Stock>> GetOutOfStockBooks();
    }
}