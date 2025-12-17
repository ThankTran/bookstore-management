using System;
using System.Collections.Generic;
using System.Linq;
using bookstore_Management.Core.Results;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;
using bookstore_Management.Services.Interfaces;

namespace bookstore_Management.Services.Implementations
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        internal AuditLogService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public Result<int> Log(AuditLog log)
        {
            try
            {
                log.ChangedDate = DateTime.Now;
                _auditLogRepository.Add(log);
                _auditLogRepository.SaveChanges();
                return Result<int>.Success(log.Id, "Đã ghi log");
            }
            catch (Exception ex)
            {
                return Result<int>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<AuditLog>> GetRecent(int take = 100)
        {
            try
            {
                var items = _auditLogRepository.GetRecent(take).ToList();
                return Result<IEnumerable<AuditLog>>.Success(items);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<AuditLog>>.Fail($"Lỗi: {ex.Message}");
            }
        }

        public Result<IEnumerable<AuditLog>> GetByEntity(string entityName, string entityId)
        {
            try
            {
                var items = _auditLogRepository.GetByEntity(entityName, entityId).ToList();
                return Result<IEnumerable<AuditLog>>.Success(items);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<AuditLog>>.Fail($"Lỗi: {ex.Message}");
            }
        }
    }
}
