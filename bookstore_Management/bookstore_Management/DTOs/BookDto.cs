using System.ComponentModel.DataAnnotations;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs
{
    /// <summary>
    /// DTO cho thêm/sửa sách từ Form
    /// Form chỉ nhập những fields này
    /// </summary>
    public class BookDto
    {
        [Required(ErrorMessage = "Tên sách không được để trống")]
        [StringLength(50, ErrorMessage = "Tên sách không quá 50 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Chưa chọn nhà cung cấp")]
        [StringLength(6, ErrorMessage = "Mã NCC không hợp lệ")]
        public string SupplierId { get; set; }
        
        [Required(ErrorMessage = "Chưa chọn thể loại")]
        public BookCategory Category { get; set; }

        [Required(ErrorMessage = "Giá nhập không được để trống")]
        [Range(0.01, 10000000, ErrorMessage = "Giá nhập phải từ 0.01 đến 10,000,000")]
        public decimal ImportPrice { get; set; }

        [Range(0, 10000, ErrorMessage = "Số lượng phải từ 0 đến 10,000")]
        public int InitialStock { get; set; } = 0;
    }
    
    /// <summary>
    /// DTO cho cập nhật giá sách
    /// </summary>
    public class UpdateBookPriceDto
    {
        [Required]
        [StringLength(6)]
        public string BookId { get; set; }

        [Required]
        [Range(0.01, 10000000)]
        public decimal NewSalePrice { get; set; }
    }

    /// <summary>
    /// DTO cho tìm kiếm sách
    /// </summary>
    public class BookSearchDto
    {
        public string Keyword { get; set; }
        public BookCategory? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string SupplierId { get; set; }
        public bool? InStockOnly { get; set; } = true;
    }

}