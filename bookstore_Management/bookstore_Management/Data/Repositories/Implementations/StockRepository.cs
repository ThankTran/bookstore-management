using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class StockRepository : Repository<Stock, (string WarehouseId, string BookId)>, IStockRepository
    {
        public StockRepository(BookstoreDbContext context) : base(context) { }

        public Stock Get(string warehouseId, string bookId)
        {
            var stock = GetById((warehouseId, bookId));
            return (stock != null && stock.DeletedDate == null) ? stock : null;
        }

        public IEnumerable<Stock> GetByBook(string bookId)
        {
            return Find(s => s.BookId == bookId && s.DeletedDate == null);
        }

        public IEnumerable<Stock> GetByWarehouse(string warehouseId)
        {
            return Find(s => s.WarehouseId == warehouseId && s.DeletedDate == null);
        }

        public int GetTotalQuantity(string bookId)
        {
            return Find(s => s.BookId == bookId && s.DeletedDate == null).Sum(s => s.StockQuantity);
        }

        public IEnumerable<Stock> GetLowStock(int threshold)
        {
            return Find(s => s.StockQuantity <= threshold && s.DeletedDate == null);
        }
    }
}