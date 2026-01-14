namespace bookstore_Management.DTOs.Common.Reports
{
    /// <summary>
    /// DTO for staff performance report response
    /// </summary>
    public class StaffPerformanceReportResponseDto
    {
        public string StaffId { get; set; }
        public string StaffName { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public decimal DailyAverageRevenue { get; set; }
    }
}

