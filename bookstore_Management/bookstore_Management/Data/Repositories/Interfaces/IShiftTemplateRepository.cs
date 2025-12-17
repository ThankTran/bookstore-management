using System.Collections.Generic;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IShiftTemplateRepository : IRepository<ShiftTemplate, string>
    {
        IEnumerable<ShiftTemplate> GetActive();
    }
}

