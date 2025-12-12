using System;
using System.Collections.Generic;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs
{
    public class StaffWorkHoursReportDto
    {
        public string StaffId { get; set; }
        public string StaffName { get; set; }
        public string WeekId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalHours { get; set; }
        public int TotalShifts { get; set; }
        public List<WorkScheduleResponseDto> Schedules { get; set; } = new List<WorkScheduleResponseDto>();
    }

    public class InventoryReportDto
    {
        public string BookId { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public BookCategory Category { get; set; }
        public int TotalStock { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal TotalValue { get; set; }
        public List<StockInfoDto> StocksByWarehouse { get; set; } = new List<StockInfoDto>();
    }

    public class SalesReportDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalDiscount { get; set; }
        public int TotalBooksSold { get; set; }
        public List<TopSellingBookDto> TopSellingBooks { get; set; } = new List<TopSellingBookDto>();
        public List<StaffSalesDto> SalesByStaff { get; set; } = new List<StaffSalesDto>();
    }

    public class TopSellingBookDto
    {
        public string BookId { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class StaffSalesDto
    {
        public string StaffId { get; set; }
        public string StaffName { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    // Bổ sung các DTO phục vụ ReportService
    public class BookSalesReport
    {
        public string BookId { get; set; }
        public string BookName { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AveragePricePerUnit { get; set; }
    }

    public class StaffPerformanceReport
    {
        public string StaffId { get; set; }
        public string StaffName { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public decimal DailyAverageRevenue { get; set; }
    }

    public class InventorySummaryReport
    {
        public int TotalBooks { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
    }

    public class CustomerSpendingReport
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalSpent { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    public class SupplierImportReport
    {
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public decimal TotalImportValue { get; set; }
        public int TotalQuantity { get; set; }
    }
}