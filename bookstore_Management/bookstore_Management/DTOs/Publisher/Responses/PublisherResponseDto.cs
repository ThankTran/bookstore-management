using System;

namespace bookstore_Management.DTOs.Publisher.Responses
{
    /// <summary>
    /// DTO for supplier response
    /// </summary>
    public class PublisherResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TotalImportBills { get; set; }
        public decimal TotalAmount { get; set; }
    }
}

