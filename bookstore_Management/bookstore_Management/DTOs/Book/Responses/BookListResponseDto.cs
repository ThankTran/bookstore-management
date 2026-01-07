using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Book.Responses
{
    /// <summary>
    /// DTO for Book ListView display
    /// Contains only fields required for ListView
    /// </summary>
    public class BookListResponseDto
    {
        /// <summary>
        /// Index (STT) - generated at Service level, NOT stored in DB
        /// </summary>
        public int Index { get; set; }

        public string BookId { get; set; }
        public string Name { get; set; }
        public string SupplierId { get; set; }
        public BookCategory Category { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? ImportPrice { get; set; }
    }
}

