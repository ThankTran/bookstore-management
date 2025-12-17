using System.Collections.Generic;
using bookstore_Management.Core.Results;
using bookstore_Management.Models;

namespace bookstore_Management.Services.Interfaces
{
    internal interface IAuditLogService
    {
        Result<int> Log(AuditLog log);
        Result<IEnumerable<AuditLog>> GetRecent(int take = 100);
        Result<IEnumerable<AuditLog>> GetByEntity(string entityName, string entityId);
    }
}

