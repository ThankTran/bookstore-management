using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IImportBillRepository : IRepository<ImportBill, string>
    {
        Task<IEnumerable<ImportBill>> GetByPublisherAsync(string publisherId);
        Task<IEnumerable<ImportBill>> GetByDateRangeAsync(DateTime start, DateTime end);
    }
}