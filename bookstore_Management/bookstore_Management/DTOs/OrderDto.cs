using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs
{
    public class OrderCreateDto
    {
        public string StaffId { get; set; }
        public string CustomerId { get; set; }
        public PaymentType PaymentMethod { get; set; }
        public decimal Discount { get; set; }
        public string Notes { get; set; }
        public List<OrderDetailCreateDto> OrderDetails { get; set; } = new List<OrderDetailCreateDto>();
    }

    public class OrderUpdateDto
    {
        public PaymentType? PaymentMethod { get; set; }
        public decimal? Discount { get; set; }
        public string Notes { get; set; }
    }

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

    public class OrderDetailCreateDto
    {
        public string BookId { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
    }

    public class OrderDetailResponseDto
    {
        public string BookId { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public decimal SalePrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
        public string Notes { get; set; }
    }
}