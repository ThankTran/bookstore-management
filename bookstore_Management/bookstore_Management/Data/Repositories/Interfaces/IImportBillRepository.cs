using System;
using System.Collections.Generic;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IImportBillRepository : IRepository<ImportBill,int>
    {
        IEnumerable<ImportBill> GetBySupplier(string supplierId);
        IEnumerable<ImportBill> GetByDateRange(DateTime start, DateTime end);
    }
}