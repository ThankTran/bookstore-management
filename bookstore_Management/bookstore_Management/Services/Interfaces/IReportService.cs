using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using bookstore_Management.Core.Results;
using bookstore_Management.DTOs.Common.Reports;

namespace bookstore_Management.Services.Interfaces
{
    public interface IReportService
    {
        // Báo cáo doanh thu
        Task<Result<decimal>> GetTotalRevenueAsync(DateTime fromDate, DateTime toDate);
        Task<Result<decimal>> GetTotalProfitAsync(DateTime fromDate, DateTime toDate);
        Task<Result<decimal>> GetAverageOrderValueAsync(DateTime fromDate, DateTime toDate);
        
        // Báo cáo đơn hàng
        Task<Result<int>> GetTotalOrderCountAsync(DateTime fromDate, DateTime toDate);
        
        // Báo cáo khách hàng mới
        Task<Result<int>> GetTotalCustomerCountAsync(DateTime fromDate, DateTime toDate);

        // Báo cáo sách bán chạy
        Task<Result<IEnumerable<BookSalesReportResponseDto>>> GetTopSellingBooksAsync(DateTime fromDate, DateTime toDate, int topCount = 10);
        Task<Result<IEnumerable<BookSalesReportResponseDto>>> GetLowestSellingBooksAsync(DateTime fromDate, DateTime toDate, int bottomCount = 5);
        
        // Báo cáo kho
        Task<Result<InventorySummaryReportResponseDto>> GetInventorySummaryAsync();
        Task<Result<decimal>> GetInventoryValueAsync();

        // Báo cáo khách hàng
        Task<Result<IEnumerable<CustomerSpendingReportResponseDto>>> GetTopSpendingCustomersAsync(int topCount = 10);
        
        // Báo cáo tỉ lệ khách hàng vãng lai và thân thiết
        Task<Result<CustomerPurchaseRatioDto>> GetWalkInToMemberPurchaseRatioAsync(DateTime fromDate, DateTime toDate);

        // Báo cáo nhà cung cấp
        Task<Result<IEnumerable<PublisherImportReportResponseDto>>> GetPublisherImportReportAsync(DateTime fromDate, DateTime toDate);
    }
}