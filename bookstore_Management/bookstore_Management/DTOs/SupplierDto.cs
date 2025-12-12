using System;
using System.ComponentModel.DataAnnotations;

namespace bookstore_Management.DTOs
{
    public class SupplierCreateDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class SupplierUpdateDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class SupplierResponseDto
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