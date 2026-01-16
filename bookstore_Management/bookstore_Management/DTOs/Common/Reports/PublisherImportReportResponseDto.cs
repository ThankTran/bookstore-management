namespace bookstore_Management.DTOs.Common.Reports
{
    /// <summary>
    /// DTO for supplier import report response
    /// </summary>
    public class PublisherImportReportResponseDto
    {
        public string PublisherId { get; set; }
        public string PublisherName { get; set; }
        public decimal TotalImportValue { get; set; }
        public int TotalQuantity { get; set; }
    }
}

