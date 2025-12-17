using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class WorkWeekRepository : Repository<WorkWeek, string>, IWorkWeekRepository
    {
        public WorkWeekRepository(BookstoreDbContext context) : base(context) { }

        public WorkWeek GetActive()
        {
            return Find(ww => ww.IsActive && ww.DeletedDate == null).FirstOrDefault();
        }

        public IEnumerable<WorkWeek> GetRecent(int take = 10)
        {
            return Find(w => w.DeletedDate == null).OrderByDescending(w => w.CreatedDate).Take(take);
        }
    }
}

