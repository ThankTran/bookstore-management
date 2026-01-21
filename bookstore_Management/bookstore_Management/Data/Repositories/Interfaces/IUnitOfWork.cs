using System;
using bookstore_Management.Data.Repositories.Interfaces;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBookRepository Books { get; }
        ICustomerRepository Customers { get; }
        IPublisherRepository Publishers { get; }
        IOrderRepository Orders { get; }
        IOrderDetailRepository OrderDetails { get; }
        IStaffRepository Staffs { get; }
        IImportBillRepository ImportBills { get; }
        IImportBillDetailRepository ImportBillDetails { get; }
        IUserRepository Users { get; }
        IAuditLogRepository AuditLogs { get; }
        
        int SaveChanges();
        int Complete();
    }
}