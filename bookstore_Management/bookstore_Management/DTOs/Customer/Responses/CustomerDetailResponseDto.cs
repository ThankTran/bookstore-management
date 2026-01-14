using System;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Customer.Responses
{
    /// <summary>
    /// DTO for Customer detail view
    /// Contains comprehensive customer information
    /// </summary>
    public class CustomerDetailResponseDto
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public MemberTier MemberLevel { get; set; }
        public decimal LoyaltyPoints { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
    }
}

