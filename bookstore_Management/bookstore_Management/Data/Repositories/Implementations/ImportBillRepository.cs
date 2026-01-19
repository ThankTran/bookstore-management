using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class ImportBillRepository
        : Repository<ImportBill, string>, IImportBillRepository
    {
        public ImportBillRepository(BookstoreDbContext context) 
            : base(context) { }

        public async Task<IEnumerable<ImportBill>> GetByPublisherAsync(string publisherId)
        {
            return await DbSet
                .Where(ib => ib.PublisherId == publisherId && ib.DeletedDate == null)
                .ToListAsync();
        }

        public async Task<IEnumerable<ImportBill>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            return await DbSet
                .Where(ib => ib.CreatedDate >= start &&
                             ib.CreatedDate <= end &&
                             ib.DeletedDate == null)
                .ToListAsync();
        }
    }
}