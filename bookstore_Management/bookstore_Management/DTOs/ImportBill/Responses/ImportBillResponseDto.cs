using System;
using System.Collections.Generic;

namespace bookstore_Management.DTOs.ImportBill.Responses
{
 
    public class ImportBillResponseDto
    {
        public string Id { get; set; }
        public string PublisherId { get; set; }
        public string PublisherName { get; set; }

        public decimal TotalAmount { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<ImportBillDetailResponseDto> ImportBillDetails { get; set; } = new List<ImportBillDetailResponseDto>();
    }
}

