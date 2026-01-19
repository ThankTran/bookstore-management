using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IOrderRepository : IRepository<Order, string>
    {
        Task<IEnumerable<Order>> GetByCustomerAsync(string customerId);
        Task<IEnumerable<Order>> GetByStaffAsync(string staffId);
        Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime start, DateTime end);
    }

}
