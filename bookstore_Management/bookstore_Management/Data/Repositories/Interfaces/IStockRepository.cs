using System.Collections.Generic;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IStockRepository : IRepository<Stock, (string WarehouseId, string BookId)>
    {
        Stock Get(string warehouseId, string bookId);
        IEnumerable<Stock> GetByBook(string bookId);
        IEnumerable<Stock> GetByWarehouse(string warehouseId);
        int GetTotalQuantity(string bookId);
        IEnumerable<Stock> GetLowStock(int threshold);
    }
}