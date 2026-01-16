using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Order.Requests
{
    
    public class UpdateOrderRequestDto
    {
        public PaymentType? PaymentMethod { get; set; }
        public decimal? Discount { get; set; }
        public string Notes { get; set; }
    }
}

