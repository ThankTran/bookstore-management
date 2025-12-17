using System.Collections.Generic;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IWorkWeekRepository : IRepository<WorkWeek, string>
    {
        WorkWeek GetActive();
        IEnumerable<WorkWeek> GetRecent(int take = 10);
    }
}

