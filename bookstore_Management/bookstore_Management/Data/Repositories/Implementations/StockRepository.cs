using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.implementations
{
    internal class StockRepository : Repository<Stock,string>, IStockRepository
    {
        public StockRepository(BookstoreDbContext context) : base(context) { }

        public Stock GetByBook(string bookId)
        {
            return Find(s => s.BookId == bookId).FirstOrDefault();
        }

        public IEnumerable<Stock> GetLowStock(int threshold)
        {
            return Find(s => s.StockQuantity <= threshold);
        }
    }
}