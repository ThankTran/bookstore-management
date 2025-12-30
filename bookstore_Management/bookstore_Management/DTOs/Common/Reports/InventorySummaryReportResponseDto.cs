namespace bookstore_Management.DTOs.Common.Reports
{
    /// <summary>
    /// DTO for inventory summary report response
    /// </summary>
    public class InventorySummaryReportResponseDto
    {
        public int TotalBooks { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
    }
}

