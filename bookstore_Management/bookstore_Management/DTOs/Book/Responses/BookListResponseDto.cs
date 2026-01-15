using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Book.Responses
{
    public class BookListResponseDto
    {
        public int Index { get; set; } // auto gen trong service
        public string BookId { get; set; }
        public string Name { get; set; }
        public string SupplierId { get; set; }
        public BookCategory Category { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? ImportPrice { get; set; }
    }
}

