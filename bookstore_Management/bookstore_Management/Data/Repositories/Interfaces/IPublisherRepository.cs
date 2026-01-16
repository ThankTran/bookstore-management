using System;
using System.Collections.Generic;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface ISupplierRepository : IRepository<Publisher,string>
    {
        IEnumerable<Publisher> SearchByName(string name);
        string GetNameBySupplierId(string supplierId);
        Publisher GetByPhone(string phone);
        Publisher GetByEmail(string email);
    }
}