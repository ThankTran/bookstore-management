using System;
using System.Collections.Generic;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Order.Responses
{
    /// <summary>
    /// DTO for order response
    /// </summary>
    public class OrderResponseDto
    {
        public string OrderId { get; set; }
        public string StaffId { get; set; }
        public string StaffName { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public PaymentType PaymentMethod { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<OrderDetailResponseDto> OrderDetails { get; set; } = new List<OrderDetailResponseDto>();
    }
}

