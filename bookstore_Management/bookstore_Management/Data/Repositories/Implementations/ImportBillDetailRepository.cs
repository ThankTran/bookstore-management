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
    internal class ImportBillDetailRepository 
        : Repository<ImportBillDetail, (string BookId, string ImportId)>, IImportBillDetailRepository
    {
        public ImportBillDetailRepository(BookstoreDbContext context) : base(context) { }

        public async Task<IEnumerable<ImportBillDetail>> GetByImportIdAsync(string importId)
        {
            return await DbSet
                .Where(ibd => ibd.ImportId == importId && ibd.DeletedDate == null)
                .ToListAsync();
        }

        public async Task<decimal> GetImportPriceByBookIdAsync(string bookId)
        {
            return await DbSet
                .Where(ibd => ibd.BookId == bookId && ibd.DeletedDate == null)
                .Select(ib => ib.ImportPrice)
                .FirstOrDefaultAsync();
        }

        public async Task SoftDeleteAsync(string importId, string bookId)
        {
            var entity = await DbSet
                .FirstOrDefaultAsync(x => x.ImportId == importId && x.BookId == bookId);

            if (entity != null)
            {
                entity.DeletedDate = DateTime.Now;
            }
        }

        public async Task<Dictionary<string, decimal?>> GetLatestImportPricesByBookIdsAsync(IEnumerable<string> bookIds)
        {
            var list = bookIds.ToList();
            if (!list.Any())
                return new Dictionary<string, decimal?>();

            var query = await DbSet
                .Where(ibd => ibd.DeletedDate == null && list.Contains(ibd.BookId))
                .Join(DbContext.Set<ImportBill>(),
                    ibd => ibd.ImportId,
                    ib => ib.Id,
                    (ibd, ib) => new { ibd.BookId, ibd.ImportPrice, ib.CreatedDate, ib.DeletedDate })
                .Where(x => x.DeletedDate == null)
                .ToListAsync();

            var result = query
                .GroupBy(x => x.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    LatestPrice = g.OrderByDescending(x => x.CreatedDate)
                                   .ThenByDescending(x => x.ImportPrice)
                                   .Select(x => (decimal?)x.ImportPrice)
                                   .FirstOrDefault()
                })
                .ToDictionary(x => x.BookId, x => x.LatestPrice);

            // Đảm bảo tất cả bookIds đều có key trong dictionary
            foreach (var id in list)
            {
                if (!result.ContainsKey(id))
                    result[id] = null;
            }

            return result;
        }
    }
}
