using System;

namespace bookstore_Management.DTOs
{
    /// <summary>
    /// DTO cho báo cáo sách bán chạy
    /// </summary>
    public class BookSalesReport
    {
        public string BookId { get; set; }
        public string BookName { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AveragePricePerUnit { get; set; }
    }

    /// <summary>
    /// DTO cho báo cáo hiệu suất nhân viên
    /// </summary>
    public class StaffPerformanceReport
    {
        public string StaffId { get; set; }
        public string StaffName { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public decimal DailyAverageRevenue { get; set; }
    }

    /// <summary>
    /// DTO cho báo cáo tồn kho
    /// </summary>
    public class InventorySummaryReport
    {
        public int TotalBooks { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
    }

    /// <summary>
    /// DTO cho báo cáo khách hàng chi tiêu nhiều
    /// </summary>
    public class CustomerSpendingReport
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalSpent { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    /// <summary>
    /// DTO cho báo cáo nhà cung cấp
    /// </summary>
    public class SupplierImportReport
    {
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int TotalImportBills { get; set; }
        public decimal TotalImportValue { get; set; }
        public int TotalBooksImported { get; set; }
    }
}