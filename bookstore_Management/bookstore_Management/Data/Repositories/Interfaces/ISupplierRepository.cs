using System;
using System.Collections.Generic;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories
{
    internal interface ISupplierRepository : IRepository<Supplier,string>
    {
        IEnumerable<Supplier> SearchByName(string name);
        Supplier GetByPhone(string phone);
        Supplier SearchByEmail(string email);
    }
}