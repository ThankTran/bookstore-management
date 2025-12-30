using System;
using System.Collections.Generic;

namespace bookstore_Management.DTOs.Common.Reports
{
    /// <summary>
    /// DTO for sales report response
    /// </summary>
    public class SalesReportResponseDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalDiscount { get; set; }
        public int TotalBooksSold { get; set; }
        public List<TopSellingBookResponseDto> TopSellingBooks { get; set; } = new List<TopSellingBookResponseDto>();
        public List<StaffSalesResponseDto> SalesByStaff { get; set; } = new List<StaffSalesResponseDto>();
    }

    /// <summary>
    /// DTO for top selling book in report
    /// </summary>
    public class TopSellingBookResponseDto
    {
        public string BookId { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }

    /// <summary>
    /// DTO for staff sales in report
    /// </summary>
    public class StaffSalesResponseDto
    {
        public string StaffId { get; set; }
        public string StaffName { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}

