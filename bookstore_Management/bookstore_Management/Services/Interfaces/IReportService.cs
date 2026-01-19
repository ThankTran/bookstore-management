using System;
using System.Collections.Generic;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs.Common.Reports;
namespace bookstore_Management.Services.Interfaces
{
    public interface IReportService
    {
        // Báo cáo doanh thu
        Result<decimal> GetTotalRevenue(DateTime fromDate, DateTime toDate); // tổng doanh thu trong khoảng
        Result<decimal> GetTotalProfit(DateTime fromDate, DateTime toDate); // tổng lợi nhuận trong khoảng
        
        Result<IEnumerable<decimal>> GetRevenue(DateTime fromDate, DateTime toDate, int jump = 1); // bước nhảy 1 ngày
        Result<IEnumerable<decimal>> GetImport(DateTime fromDate, DateTime toDate, int jump = 1); 
        
        
        Result<decimal> GetAverageOrderValue(DateTime fromDate, DateTime toDate);
        // Báo cáo đơn hàng
        Result<int> GetTotalOrderCount(DateTime fromDate, DateTime toDate);
        
        // Báo cáo khách hàng mới
        Result<int> GetTotalCustomerCount(DateTime fromDate, DateTime toDate);

        // Báo cáo sách bán chạy
        Result<IEnumerable<BookSalesReportResponseDto>> GetTopSellingBooks(DateTime fromDate, DateTime toDate, int topCount = 10);
        Result<IEnumerable<BookSalesReportResponseDto>> GetLowestSellingBooks(DateTime fromDate, DateTime toDate, int bottomCount = 5);
        
        
        // Báo cáo kho
        Result<InventorySummaryReportResponseDto> GetInventorySummary();
        Result<decimal> GetInventoryValue();

        // Báo cáo khách hàng
        Result<IEnumerable<CustomerSpendingReportResponseDto>> GetTopSpendingCustomers(int topCount = 10);
        
        // Báo cáo tỉ lệ khách hàng vãng lai và thân thiết
        Result<CustomerPurchaseRatioDto> GetWalkInToMemberPurchaseRatio(DateTime fromDate, DateTime toDate);

        // Báo cáo nhà cung cấp
        Result<IEnumerable<PublisherImportReportResponseDto>> GetPublisherImportReport(DateTime fromDate, DateTime toDate);
    }
}