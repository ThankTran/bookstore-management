using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Book.Responses
{
    public class BookDetailResponseDto // readonly
    {
        public string BookId { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public BookCategory Category { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal ImportPrice { get; set; }

        public string PublisherId { get; set; }
        public int StockQuantity { get; set; }
        public string PublisherName { get; set; }
        
        
    }
}

