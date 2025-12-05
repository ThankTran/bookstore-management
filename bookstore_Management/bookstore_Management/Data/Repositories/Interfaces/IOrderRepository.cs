using System;
using System.Collections.Generic;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IOrderRepository : IRepository<Order,string>
    {
        IEnumerable<Order> GetByCustomer(string customerId);
        IEnumerable<Order> GetByStaff(string staffId);
        IEnumerable<Order> GetByDateRange(DateTime start, DateTime end);
    }
}
