using System.Collections.Generic;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IOrderDetailRepository : IRepository<OrderDetail,int>
    {
        IEnumerable<OrderDetail> GetByOrder(string orderId);
        IEnumerable<OrderDetail> GetByBook(string bookId);
    }
}