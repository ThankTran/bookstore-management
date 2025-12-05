using System.Collections.Generic;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IStockRepository : IRepository<Stock,string>
    {
        Stock GetByBook(string bookId);
        IEnumerable<Stock> GetLowStock(int threshold);
    }
}