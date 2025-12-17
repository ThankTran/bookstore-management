using System;
using System.ComponentModel.DataAnnotations;

namespace bookstore_Management.DTOs
{
    public class StockCreateDto
    {
        public string WarehouseId { get; set; }
        public string BookId { get; set; }
        public int StockQuantity { get; set; }
    }

    public class StockUpdateDto
    {
        public int StockQuantity { get; set; }
    }

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