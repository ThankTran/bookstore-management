using System.Collections.Generic;

namespace bookstore_Management.DTOs.ImportBill.Requests
{
    
    public class CreateImportBillRequestDto
    {
        public string PublisherId { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public List<ImportBillDetailCreateRequestDto> ImportBillDetails { get; set; } = new List<ImportBillDetailCreateRequestDto>();
    }
}

