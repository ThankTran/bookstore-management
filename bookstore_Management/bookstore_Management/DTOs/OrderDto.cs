using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs
{
    /// <summary>
    /// DTO cho tạo đơn hàng mới
    /// </summary>
    public class OrderDto
    {

        public string StaffId { get; set; }
        public string CustomerId { get; set; } 
        public PaymentType PaymentMethod { get; set; }
        public decimal DiscountPercent { get; set; } = 0;
        public string Notes { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }

    /// <summary>
    /// DTO cho từng item trong đơn hàng
    /// </summary>
    public class OrderItemDto
    {
        public string BookId { get; set; } 
        public int Quantity { get; set; }
    }

    /// <summary>
    /// DTO cho cập nhật đơn hàng
    /// </summary>
    public class UpdateOrderDto
    {
        public string Notes { get; set; }
        public PaymentType? PaymentMethod { get; set; }
        public decimal? Discount { get; set; }
    }

    /// <summary>
    /// DTO cho tìm kiếm đơn hàng
    /// </summary>
    public class OrderSearchDto
    {
        public string StaffId { get; set; }
        public string CustomerId { get; set; }
        public PaymentType? PaymentMethod { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinTotal { get; set; }
        public decimal? MaxTotal { get; set; }
    }
}