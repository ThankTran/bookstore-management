using System;
using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class ImportBillDetailRepository : Repository<ImportBillDetail, (string BookId, string ImportId)>, IImportBillDetailRepository
    {
        public ImportBillDetailRepository(BookstoreDbContext context) : base(context) { }

        public IEnumerable<ImportBillDetail> GetByImportId(string importId)
        {
            return Find(ibd => ibd.ImportId == importId && ibd.DeletedDate == null);
        }
        
        public void SoftDelete(string importId, string bookId)
        {
            var entity = _dbSet.FirstOrDefault(x => x.ImportId == importId && x.BookId == bookId);
            if (entity != null)
            {
                entity.DeletedDate = DateTime.Now;
            }
        }
    }
}

