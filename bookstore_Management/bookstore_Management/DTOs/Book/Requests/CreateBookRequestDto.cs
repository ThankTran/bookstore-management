using bookstore_Management.Core.Enums;

namespace bookstore_Management.DTOs.Book.Requests
{
    
    public class CreateBookRequestDto
    {
        public string Id { get; set; }  // xóa
        public string Name { get; set; }
        public string Author { get; set; } 
        public BookCategory Category { get; set; }
        public decimal? SalePrice { get; set; }
        public string PublisherName { get; set; } // combo box
    }
}

