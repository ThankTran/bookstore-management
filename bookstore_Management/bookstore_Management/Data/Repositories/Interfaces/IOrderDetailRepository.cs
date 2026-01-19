using System.Collections.Generic;
using System.Threading.Tasks;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IOrderDetailRepository : IRepository<OrderDetail, (string BookId, string OrderId)>
    {
        Task<IEnumerable<OrderDetail>> GetByOrderAsync(string orderId);
        Task<IEnumerable<OrderDetail>> GetByBookAsync(string bookId);
    }
}