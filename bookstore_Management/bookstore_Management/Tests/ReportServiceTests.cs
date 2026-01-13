//using System;
//using bookstore_Management.Data.Context;
//using bookstore_Management.Data.Repositories.implementations;
//using bookstore_Management.Data.Repositories.Implementations;
//using bookstore_Management.Services.Implementations;
//using NUnit.Framework;

//namespace bookstore_Management.Tests
//{
//    [TestFixture]
//    public class ReportServiceTests
//    {
//        private BookstoreDbContext _context;
//        private ReportService _service;

//        [SetUp]
//        public void SetUp()
//        {
//            _context = new BookstoreDbContext();
//            _service = new ReportService(
//                new OrderRepository(_context),
//                new OrderDetailRepository(_context),
//                new StockRepository(_context),
//                new BookRepository(_context),
//                new CustomerRepository(_context),
//                new StaffRepository(_context),
//                new ImportBillRepository(_context),
//                new ImportBillDetailRepository(_context)
//            );
//        }

//        [Test]
//        public void GetTotalRevenue_ShouldReturnNonNegative()
//        {
//            var result = _service.GetTotalRevenue(DateTime.Today.AddMonths(-1), DateTime.Today);
//            Assert.IsTrue(result.IsSuccess);
//            Assert.GreaterOrEqual(result.Data, 0);
//        }
//    }
//}

