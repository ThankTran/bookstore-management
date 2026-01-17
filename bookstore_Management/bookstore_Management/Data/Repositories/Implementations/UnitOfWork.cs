using System;
using bookstore_Management.Data.Context;
//using bookstore_Management.Data.Repositories.implementations;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.Data.Repositories.Interfaces;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly BookstoreDbContext _context;

        // Dùng public thay vì internal để có thể truy cập từ bên ngoài
        public IBookRepository Books { get; }
        public ICustomerRepository Customers { get; }
        public IPublisherRepository Publishers { get; }
        public IOrderRepository Orders { get; }
        public IOrderDetailRepository OrderDetails { get; }
        public IStaffRepository Staffs { get; }
        public IImportBillRepository ImportBills { get; }
        public IImportBillDetailRepository ImportBillDetails { get; }
        public IUserRepository Users { get; }
        public IAuditLogRepository AuditLogs { get; }
        public UnitOfWork(BookstoreDbContext context)
        {
            _context = context;

            // Khởi tạo tất cả repositories với cùng 1 DbContext
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

        // Phương thức lưu tất cả thay đổi
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        // Thêm phương thức Complete (alias của SaveChanges)
        public int Complete()
        {
            return SaveChanges();
        }

        // Giải phóng resources
        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}