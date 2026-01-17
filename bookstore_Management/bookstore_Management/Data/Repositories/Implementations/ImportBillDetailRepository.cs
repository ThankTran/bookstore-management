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

        public decimal GetImportPriceByBookId(string bookId)
        {
            return Find(ibd => ibd.BookId == bookId && ibd.DeletedDate == null)
                .Select(ib => ib.ImportPrice)
                .FirstOrDefault();
        }
        
        public void SoftDelete(string importId, string bookId)
        {
            var entity = _dbSet.FirstOrDefault(x => x.ImportId == importId && x.BookId == bookId);
            if (entity != null)
            {
                entity.DeletedDate = DateTime.Now;
            }
        }

        public Dictionary<string, decimal?> GetLatestImportPricesByBookIds(IEnumerable<string> bookIds)
        {
            var bookIdList = bookIds.ToList();
            if (!bookIdList.Any())
                return new Dictionary<string, decimal?>();

            var result = _dbSet
                .Where(ibd => ibd.DeletedDate == null && bookIdList.Contains(ibd.BookId))
                .Join(_dbContext.Set<ImportBill>(),
                    ibd => ibd.ImportId,
                    ib => ib.Id,
                    (ibd, ib) => new { ibd.BookId, ibd.ImportPrice, ib.CreatedDate, ib.DeletedDate })
                .Where(x => x.DeletedDate == null)
                .GroupBy(x => x.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    LatestPrice = g.OrderByDescending(x => x.CreatedDate)
                        .ThenByDescending(x => x.ImportPrice)
                        .Select(x => (decimal?)x.ImportPrice)
                        .FirstOrDefault()
                })
                .ToList()
                .ToDictionary(x => x.BookId, x => x.LatestPrice);

            // Ensure all bookIds are in the dictionary (with null if no import found)
            foreach (var bookId in bookIdList)
            {
                if (!result.ContainsKey(bookId))
                    result[bookId] = null;
            }

            return result;
        }
    }
}

