using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Customer.Requests
{
    
    public class UpdateCustomerRequestDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public MemberTier? MemberLevel { get; set; }
        public decimal? LoyaltyPoints { get; set; }
    }
}

