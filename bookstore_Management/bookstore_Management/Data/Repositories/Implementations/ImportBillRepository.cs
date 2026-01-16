using System;
using System.Collections.Generic;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class ImportBillRepository : Repository<ImportBill,string>, IImportBillRepository
    {
        public ImportBillRepository(BookstoreDbContext context) : base(context) { }

        public IEnumerable<ImportBill> GetByPublisher(string publisherId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ImportBill> GetByDateRange(DateTime start, DateTime end)
        {
            return Find(ib => ib.CreatedDate >= start && ib.CreatedDate <= end);
        }
    }
}