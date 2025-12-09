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