namespace bookstore_Management.DTOs.Common.Reports
{
    /// <summary>
    /// DTO for book sales report response
    /// </summary>
    public class BookSalesReportResponseDto
    {
        public string BookId { get; set; }
        public string BookName { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AveragePricePerUnit { get; set; }
    }
}

