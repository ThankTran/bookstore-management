using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Book.Requests
{
    /// <summary>
    /// DTO for creating a new book
    /// </summary>
    public class CreateBookRequestDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public BookCategory Category { get; set; }
        public decimal? SalePrice { get; set; }
        public string SupplierId { get; set; }
    }
}

