using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Book.Responses
{
    public class BookDetailResponseDto
    {
        public string BookId { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public BookCategory Category { get; set; }
        public decimal? SalePrice { get; set; }
        public string SupplierName { get; set; }

        public BookDetailResponseDto(string bookId, string name, string author, BookCategory category, decimal? salePrice, string supplierName)
        {
            BookId = bookId;
            Name = name;
            Author = author;
            Category = category;
            SalePrice = salePrice;
            SupplierName = supplierName;
        }
        
    }
}

