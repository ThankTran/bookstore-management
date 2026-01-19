using System;
using System.Threading.Tasks;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly BookstoreDbContext _context;

        public IBookRepository Books { get; }
        public ICustomerRepository Customers { get; }
        public IPublisherRepository Publishers { get; }
        public IOrderRepository Orders { get; }
        public IOrderDetailRepository OrderDetails { get; }
        public IStaffRepository Staffs { get; }
        public IImportBillRepository ImportBills { get; }
        public IImportBillDetailRepository ImportBillDetails { get; }
        public IUserRepository Users { get; }

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
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int Complete()
        {
            return SaveChanges();
        }

        public async Task<int> CompleteAsync()
        {
            return await SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}