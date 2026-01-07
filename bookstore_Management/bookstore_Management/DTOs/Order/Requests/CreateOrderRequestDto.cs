using System.Collections.Generic;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Order.Requests
{
    /// <summary>
    /// DTO for creating a new order
    /// </summary>
    public class CreateOrderRequestDto
    {
        public string StaffId { get; set; }
        public string CustomerId { get; set; }
        public PaymentType PaymentMethod { get; set; }
        public decimal Discount { get; set; }
        public string Notes { get; set; }
        public List<OrderDetailCreateRequestDto> OrderDetails { get; set; } = new List<OrderDetailCreateRequestDto>();
    }
}

