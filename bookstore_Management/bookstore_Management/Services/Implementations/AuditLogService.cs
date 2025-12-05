/*using System;
using bookstore_Management.Models;

namespace bookstore_Management.Services.Implementations
{
    /// <summary>
    /// Service để ghi audit log
    /// </summary>
    public class AuditLogService
    {
        private readonly ApplicationDbContext _context;

        public AuditLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Ghi audit log khi tạo mới entity
        /// </summary>
        public async System.Threading.Tasks.Task LogCreateAsync(
            string entityName, 
            string entityId, 
            object newValues, 
            string changedBy,
            string description = null)
        {
            var auditLog = new AuditLog
            {
                EntityName = entityName,
                EntityId = entityId,
                Action = "Create",
                OldValues = null,
                NewValues = JsonConvert.SerializeObject(newValues),
                ChangedBy = changedBy,
                ChangedDate = DateTime.Now,
                Description = description ?? $"Tạo mới {entityName}"
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Ghi audit log khi cập nhật entity
        /// </summary>
        public async System.Threading.Tasks.Task LogUpdateAsync(
            string entityName, 
            string entityId, 
            object oldValues, 
            object newValues, 
            string changedBy,
            string description = null)
        {
            var auditLog = new AuditLog
            {
                EntityName = entityName,
                EntityId = entityId,
                Action = "Update",
                OldValues = JsonConvert.SerializeObject(oldValues),
                NewValues = JsonConvert.SerializeObject(newValues),
                ChangedBy = changedBy,
                ChangedDate = DateTime.Now,
                Description = description ?? $"Cập nhật {entityName}"
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Ghi audit log khi xóa entity
        /// </summary>
        public async System.Threading.Tasks.Task LogDeleteAsync(
            string entityName, 
            string entityId, 
            object deletedValues, 
            string changedBy,
            string description = null)
        {
            var auditLog = new AuditLog
            {
                EntityName = entityName,
                EntityId = entityId,
                Action = "Delete",
                OldValues = JsonConvert.SerializeObject(deletedValues),
                NewValues = null,
                ChangedBy = changedBy,
                ChangedDate = DateTime.Now,
                Description = description ?? $"Xóa {entityName}"
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
    }
}
?*/