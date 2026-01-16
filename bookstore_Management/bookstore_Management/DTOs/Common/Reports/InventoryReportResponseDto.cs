using System.Collections.Generic;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Common.Reports
{
    /// <summary>
    /// DTO for inventory report response
    /// </summary>
    public class InventoryReportResponseDto
    {
        public string BookId { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public BookCategory Category { get; set; }
        public int TotalStock { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal TotalValue { get; set; }
    }
}

