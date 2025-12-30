using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Customer.Responses
{
    /// <summary>
    /// DTO for Customer ListView display
    /// Contains only fields required for ListView
    /// </summary>
    public class CustomerListResponseDto
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public MemberTier MemberLevel { get; set; }
        public decimal LoyaltyPoints { get; set; }
    }
}

