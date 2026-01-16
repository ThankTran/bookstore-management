using System;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.implementations;
using bookstore_Management.Data.Repositories.Interfaces;

namespace bookstore_Management.Data.Repositories.Implementations
{
    public class UnitOfWork : IDisposable
    {
        private readonly BookstoreDbContext _context;

        internal IBookRepository Books { get; }
        internal ICustomerRepository Customers { get; }
        internal IPublisherRepository Publishers { get; }
        internal IOrderRepository Orders { get; }
        internal IOrderDetailRepository OrderDetails { get; }
        internal IStaffRepository Staffs { get; }
        internal IImportBillRepository ImportBills { get; }
        internal IImportBillDetailRepository ImportBillDetails { get; }
        internal IUserRepository Users { get; }
        internal IAuditLogRepository AuditLogs { get; }

        public UnitOfWork(BookstoreDbContext context)
        {
            _context = context;

            Books = new BookRepository(_context);
            Customers = new CustomerRepository(_context);
            Publishers = new PublisherRepository(_context);
            Orders = new OrderRepository(_context);
            OrderDetails = new OrderDetailRepository(_context);
            Staffs = new StaffRepository(_context);
            ImportBills = new ImportBillRepository(_context);
            ImportBillDetails = new ImportBillDetailRepository(_context);
            Users = new UserRepository(_context);
            AuditLogs = new AuditLogRepository(_context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}