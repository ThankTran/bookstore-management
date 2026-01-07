namespace bookstore_Management.DTOs.Common.Reports
{
    /// <summary>
    /// DTO for supplier import report response
    /// </summary>
    public class SupplierImportReportResponseDto
    {
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public decimal TotalImportValue { get; set; }
        public int TotalQuantity { get; set; }
    }
}

