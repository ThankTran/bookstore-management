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
        [Required(ErrorMessage = "Mã nhân viên không được để trống")]
        [StringLength(6)]
        public string StaffId { get; set; }

        [StringLength(6)]
        public string CustomerId { get; set; }  // Null nếu khách vãng lai

        [Required(ErrorMessage = "Phương thức thanh toán không được để trống")]
        public PaymentType PaymentMethod { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Discount { get; set; } = 0;

        [StringLength(500)]
        public string Notes { get; set; }

        [Required(ErrorMessage = "Danh sách sản phẩm không được để trống")]
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }

    /// <summary>
    /// DTO cho từng item trong đơn hàng
    /// </summary>
    public class OrderItemDto
    {
        [Required(ErrorMessage = "Mã sách không được để trống")]
        [StringLength(6)]
        public string BookId { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải >= 1")]
        public int Quantity { get; set; }
    }

    /// <summary>
    /// DTO cho cập nhật đơn hàng
    /// </summary>
    public class UpdateOrderDto
    {
        [StringLength(500)]
        public string Notes { get; set; }

        public PaymentType? PaymentMethod { get; set; }

        [Range(0, double.MaxValue)]
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