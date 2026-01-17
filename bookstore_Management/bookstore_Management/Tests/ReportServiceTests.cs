// using NUnit.Framework;
// using bookstore_Management.Services.Interfaces;
// using System;
//
// namespace bookstore_Management.Tests
// {
//     [TestFixture]
//     public class ReportServiceTests
//     {
//         [SetUp]
//         public void Setup()
//         {
//             Console.WriteLine("[ReportServiceTest] Đã kết nối database");
//         }
//
//         [Test]
//         public void GetTotalRevenue_Should_ReturnGreaterThanZero_When_DataExist()
//         {
//             var service = TestHelper.GetReportService();
//             var from = DateTime.Now.AddYears(-1);
//             var to = DateTime.Now;
//             var result = service.GetTotalRevenue(from, to);
//             Assert.That(result.IsSuccess && result.Data >= 0, $"{result.ErrorMessage}");
//             Console.WriteLine($"Tổng doanh thu: {result.Data:#,##0}");
//         }
//
//         [Test]
//         public void GetTotalProfit_Should_NotThrowException()
//         {
//             var service = TestHelper.GetReportService();
//             var from = DateTime.Now.AddYears(-1);
//             var to = DateTime.Now;
//             Assert.DoesNotThrow(() => { var r = service.GetTotalProfit(from, to); Console.WriteLine($"Profit: {r.Data}"); });
//         }
//
//         [Test]
//         public void GetInventorySummary_Should_ReturnInventory()
//         {
//             var service = TestHelper.GetReportService();
//             var result = service.GetInventorySummary();
//             Assert.IsTrue(result.IsSuccess);
//             Console.WriteLine($"Tồn kho tổng: {result.Data.TotalBooks}, Giá trị: {result.Data.TotalValue}");
//         }
//     }
// }
