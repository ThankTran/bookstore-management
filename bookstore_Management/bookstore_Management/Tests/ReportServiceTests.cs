using NUnit.Framework;
using bookstore_Management.Services.Interfaces;
using System;
using System.Data.Entity;
using System.Threading.Tasks;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Implementations;
using bookstore_Management.DTOs.Common.Reports;
using bookstore_Management.Services.Implementations;

namespace bookstore_Management.Tests
{
    [TestFixture]
    public class ReportServiceTests
    {
        [SetUp]
        public void Setup()
        {
            Console.WriteLine("[ReportServiceTest] Đã kết nối database");
        }

        [Test]
        public async Task GetInventorySummary_Should_ReturnInventory()
        {
            var context = new BookstoreDbContext();
            var unitOfWork = new UnitOfWork(context);
            var service = new ReportService(unitOfWork);

            var result = await service.GetInventorySummaryAsync();
            
            Assert.IsTrue(result.IsSuccess);
            Console.WriteLine($"Tồn kho tổng: {result.Data.TotalBooks}\n Hàng thấp: {result.Data.LowStockCount} ");
        }
    }
}
