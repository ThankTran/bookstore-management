namespace bookstore_Management.DTOs.Common.Reports
{
    /// <summary>
    /// DTO for customer spending report response
    /// </summary>
    public class CustomerSpendingReportResponseDto
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalSpent { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
    }
}

