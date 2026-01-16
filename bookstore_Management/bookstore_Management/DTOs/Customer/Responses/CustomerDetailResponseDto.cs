using System;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Customer.Responses
{
    
    public class CustomerDetailResponseDto
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public MemberTier MemberLevel { get; set; }
        public decimal LoyaltyPoints { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
    }
}

