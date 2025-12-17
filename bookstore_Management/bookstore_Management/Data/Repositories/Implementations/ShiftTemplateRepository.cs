using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class ShiftTemplateRepository : Repository<ShiftTemplate, string>, IShiftTemplateRepository
    {
        public ShiftTemplateRepository(BookstoreDbContext context) : base(context) { }

        public IEnumerable<ShiftTemplate> GetActive()
        {
            return Find(st => st.DeletedDate == null);
        }
    }
}

