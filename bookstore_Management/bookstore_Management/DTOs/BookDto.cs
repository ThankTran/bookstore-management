using System;
using System.Collections.Generic;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs
{
    public class BookCreateDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public BookCategory Category { get; set; }
        public decimal? SalePrice { get; set; }
        public string SupplierId { get; set; }
    }

    public class BookUpdateDto
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public BookCategory? Category { get; set; }
        public decimal? SalePrice { get; set; }
        public string SupplierId { get; set; }
    }

    public class BookResponseDto
    {
        public string BookId { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public BookCategory Category { get; set; }
        public decimal? SalePrice { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public List<StockInfoDto> Stocks { get; set; } = new List<StockInfoDto>();
        public int TotalStock { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class StockInfoDto
    {
        public string WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int Quantity { get; set; }
    }
}