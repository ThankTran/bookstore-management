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
        internal ISupplierRepository Suppliers { get; }
        internal IOrderRepository Orders { get; }
        internal IOrderDetailRepository OrderDetails { get; }
        internal IStaffRepository Staffs { get; }
        internal IStockRepository Stocks { get; }
        internal IImportBillRepository ImportBills { get; }
        internal IImportBillDetailRepository ImportBillDetails { get; }
        internal IShiftTemplateRepository ShiftTemplates { get; }
        internal IWorkWeekRepository WorkWeeks { get; }
        internal IStaffShiftRegistrationRepository StaffShiftRegistrations { get; }
        internal IWorkScheduleRepository WorkSchedules { get; }
        internal IUserRepository Users { get; }
        internal IAuditLogRepository AuditLogs { get; }

        public UnitOfWork(BookstoreDbContext context)
        {
            _context = context;

            Books = new BookRepository(_context);
            Customers = new CustomerRepository(_context);
            Suppliers = new SupplierRepository(_context);
            Orders = new OrderRepository(_context);
            OrderDetails = new OrderDetailRepository(_context);
            Staffs = new StaffRepository(_context);
            Stocks = new StockRepository(_context);
            ImportBills = new ImportBillRepository(_context);
            ImportBillDetails = new ImportBillDetailRepository(_context);
            ShiftTemplates = new ShiftTemplateRepository(_context);
            WorkWeeks = new WorkWeekRepository(_context);
            StaffShiftRegistrations = new StaffShiftRegistrationRepository(_context);
            WorkSchedules = new WorkScheduleRepository(_context);
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