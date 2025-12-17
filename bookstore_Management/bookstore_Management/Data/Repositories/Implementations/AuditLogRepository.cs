using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class AuditLogRepository : Repository<AuditLog, int>, IAuditLogRepository
    {
        public AuditLogRepository(BookstoreDbContext context) : base(context) { }

        public IEnumerable<AuditLog> GetByEntity(string entityName, string entityId)
        {
            return Find(a => a.EntityName == entityName && a.EntityId == entityId);
        }

        public IEnumerable<AuditLog> GetRecent(int take = 100)
        {
            return GetAll().OrderByDescending(a => a.ChangedDate).Take(take);
        }
    }
}

