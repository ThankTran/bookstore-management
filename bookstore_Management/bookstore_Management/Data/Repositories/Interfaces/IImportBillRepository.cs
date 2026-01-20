using System;
using System.Collections.Generic;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    public interface IImportBillRepository : IRepository<ImportBill,string>
    {
        IEnumerable<ImportBill> GetByPublisher(string publisherId);
        IEnumerable<ImportBill> GetByDateRange(DateTime start, DateTime end);
    }
}