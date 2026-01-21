using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    public interface IImportBillDetailRepository : IRepository<ImportBillDetail, (string BookId, string ImportId)>
    {
        IEnumerable<ImportBillDetail> GetByImportId(string importId);
        decimal GetImportPriceByBookId(string bookId);
        void SoftDelete(string importId, string bookId);
        
        /// <summary>
        /// Gets the latest import price for each book (for multiple books)
        /// Returns dictionary: BookId -> Latest ImportPrice
        /// </summary>
        Dictionary<string, decimal?> GetLatestImportPricesByBookIds(IEnumerable<string> bookIds);
    }
}

