using System.Collections.Generic;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IImportBillDetailRepository : IRepository<ImportBillDetail, (string BookId, string ImportId)>
    {
        IEnumerable<ImportBillDetail> GetByImportId(string importId);
        void SoftDelete(string importId, string bookId);
    }
}

