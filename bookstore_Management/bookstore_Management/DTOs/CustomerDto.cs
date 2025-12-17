using System;
using System.ComponentModel.DataAnnotations;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs
{
    
    public class CustomerCreateDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
    }

    public class CustomerUpdateDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public MemberTier? MemberLevel { get; set; }
        public decimal? LoyaltyPoints { get; set; }
    }

    public class CustomerResponseDto
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public MemberTier MemberLevel { get; set; }
        public decimal LoyaltyPoints { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
    }
}