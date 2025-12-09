using System;
using System.Collections.Generic;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs;
namespace bookstore_Management.Services.Interfaces
{
    public interface IReportService
    {
// Báo cáo doanh thu
        Result<decimal> GetTotalRevenue(DateTime fromDate, DateTime toDate);
        Result<decimal> GetTotalProfit(DateTime fromDate, DateTime toDate);
        Result<decimal> GetAverageOrderValue(DateTime fromDate, DateTime toDate);
        // Báo cáo đơn hàng
        Result<int> GetTotalOrderCount(DateTime fromDate, DateTime toDate);
        Result<decimal> GetTotalDiscountGiven(DateTime fromDate, DateTime toDate);

        // Báo cáo sách bán chạy
        Result<IEnumerable<BookSalesReport>> GetTopSellingBooks(DateTime fromDate, DateTime toDate, int topCount = 10);
        Result<IEnumerable<BookSalesReport>> GetLowestSellingBooks(DateTime fromDate, DateTime toDate, int bottomCount = 5);

        // Báo cáo nhân viên
        Result<IEnumerable<StaffPerformanceReport>> GetStaffPerformance(DateTime fromDate, DateTime toDate);
        Result<StaffPerformanceReport> GetStaffPerformanceDetail(string staffId, DateTime fromDate, DateTime toDate);

        // Báo cáo kho
        Result<InventorySummaryReport> GetInventorySummary();
        Result<decimal> GetInventoryValue();

        // Báo cáo khách hàng
        Result<IEnumerable<CustomerSpendingReport>> GetTopSpendingCustomers(int topCount = 10);
        Result<int> GetNewCustomersCount(DateTime fromDate, DateTime toDate);

        // Báo cáo nhà cung cấp
        Result<IEnumerable<SupplierImportReport>> GetSupplierImportReport(DateTime fromDate, DateTime toDate);
    }
}