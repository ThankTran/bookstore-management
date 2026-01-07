using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Customer.Requests
{
    /// <summary>
    /// DTO for updating an existing customer
    /// </summary>
    public class UpdateCustomerRequestDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public MemberTier? MemberLevel { get; set; }
        public decimal? LoyaltyPoints { get; set; }
    }
}

