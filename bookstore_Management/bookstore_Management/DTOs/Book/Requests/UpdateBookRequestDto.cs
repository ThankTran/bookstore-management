using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Book.Requests
{
    /// <summary>
    /// DTO for updating an existing book
    /// </summary>
    public class UpdateBookRequestDto
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public BookCategory? Category { get; set; }
        public decimal? SalePrice { get; set; }
        public string SupplierId { get; set; }
    }
}

