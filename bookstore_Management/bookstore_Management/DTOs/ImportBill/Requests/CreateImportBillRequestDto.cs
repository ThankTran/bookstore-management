using System.Collections.Generic;

namespace bookstore_Management.DTOs.ImportBill.Requests
{
    /// <summary>
    /// DTO for creating a new import bill
    /// </summary>
    public class CreateImportBillRequestDto
    {
        public string SupplierId { get; set; }
        public string WarehouseId { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public List<ImportBillDetailCreateRequestDto> ImportBillDetails { get; set; } = new List<ImportBillDetailCreateRequestDto>();
    }
}

