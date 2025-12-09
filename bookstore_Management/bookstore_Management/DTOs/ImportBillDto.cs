using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace bookstore_Management.DTOs
{
    /// <summary>
    /// DTO cho tạo/sửa hóa đơn nhập
    /// </summary>
    public class ImportBillDto
    {

        public string ImportBillCode { get; set; }
        public DateTime ImportDate { get; set; }
        public string SupplierId { get; set; }
        public string Notes { get; set; }
        public List<ImportBillDetailDto> Items { get; set; } = new List<ImportBillDetailDto>();
    }

    /// <summary>
    /// DTO cho từng item trong hóa đơn nhập
    /// </summary>
    public class ImportBillDetailDto
    {
        [Required(ErrorMessage = "Mã sách không được để trống")]
        [StringLength(6)]
        public string BookId { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải >= 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Giá nhập không được để trống")]
        [Range(0, double.MaxValue)]
        public decimal ImportPrice { get; set; }
    }

    /// <summary>
    /// DTO cho tìm kiếm hóa đơn nhập
    /// </summary>
    public class ImportBillSearchDto
    {
        public string ImportBillCode { get; set; }
        public string SupplierId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinTotal { get; set; }
        public decimal? MaxTotal { get; set; }
    }
}