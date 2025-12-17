using System.Collections.Generic;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IAuditLogRepository : IRepository<AuditLog, int>
    {
        IEnumerable<AuditLog> GetByEntity(string entityName, string entityId);
        IEnumerable<AuditLog> GetRecent(int take = 100);
    }
}

