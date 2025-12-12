using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace bookstore_Management.DTOs
{
    public class ImportBillCreateDto
    {
        public string SupplierId { get; set; }
        public string WarehouseId { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public List<ImportBillDetailCreateDto> ImportBillDetails { get; set; } = new List<ImportBillDetailCreateDto>();
    }

    public class ImportBillUpdateDto
    {
        public string Notes { get; set; }
    }

    public class ImportBillResponseDto
    {
        public string Id { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<ImportBillDetailResponseDto> ImportBillDetails { get; set; } = new List<ImportBillDetailResponseDto>();
    }

    public class ImportBillDetailCreateDto
    {
        public string BookId { get; set; }
        public int Quantity { get; set; }
        public decimal ImportPrice { get; set; }
    }

    public class ImportBillDetailResponseDto
    {
        public string BookId { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public int Quantity { get; set; }
        public decimal ImportPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}