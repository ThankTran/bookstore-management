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
        public string Name { get; set; }
        public string SupplierId { get; set; }
        public BookCategory Category { get; set; }
        public decimal ImportPrice { get; set; }
        public int InitialStock { get; set; } = 0;
    }
    
    /// <summary>
    /// DTO cho cập nhật giá sách
    /// </summary>
    public class UpdateBookPriceDto
    {
        
        public string BookId { get; set; }
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