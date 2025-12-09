using System;
using System.Collections.Generic;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class ImportBillRepository : Repository<ImportBill,int>, IImportBillRepository
    {
        public ImportBillRepository(BookstoreDbContext context) : base(context) { }

        public IEnumerable<ImportBill> GetBySupplier(string supplierId)
        {
            return Find(ib => ib.SupplierId == supplierId);
        }

        public IEnumerable<ImportBill> GetByDateRange(DateTime start, DateTime end)
        {
            return Find(ib => ib.ImportDate >= start && ib.ImportDate <= end);
        }
    }
}