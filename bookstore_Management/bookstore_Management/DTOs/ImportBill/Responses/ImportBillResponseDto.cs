using System;
using System.Collections.Generic;

namespace bookstore_Management.DTOs.ImportBill.Responses
{
 
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
}

