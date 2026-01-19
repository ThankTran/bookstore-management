using System.Collections.Generic;
using System.Threading.Tasks;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IImportBillDetailRepository 
        : IRepository<ImportBillDetail, (string BookId, string ImportId)>
    {
        Task<IEnumerable<ImportBillDetail>> GetByImportIdAsync(string importId);
        Task<decimal> GetImportPriceByBookIdAsync(string bookId);
        Task SoftDeleteAsync(string importId, string bookId);
        Task<Dictionary<string, decimal?>> GetLatestImportPricesByBookIdsAsync(IEnumerable<string> bookIds);
    }
}