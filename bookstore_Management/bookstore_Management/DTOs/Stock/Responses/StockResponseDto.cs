using System;

namespace bookstore_Management.DTOs.Stock.Responses
{
    /// <summary>
    /// DTO for stock response
    /// </summary>
    public class StockResponseDto
    {
        public string WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string BookId { get; set; }
        public string BookName { get; set; }
        public int StockQuantity { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

