using System.Collections.Generic;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class OrderDetailRepository : Repository<OrderDetail, (string BookId, string OrderId)>, IOrderDetailRepository
    {
        public OrderDetailRepository(BookstoreDbContext context) : base(context) { }

        public IEnumerable<OrderDetail> GetByOrder(string orderId)
        {
            return Find(od => od.OrderId == orderId);
        }

        public IEnumerable<OrderDetail> GetByBook(string bookId)
        {
            return Find(od => od.BookId == bookId);
        }
    }
}