using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using bookstore_Management.Core.Enums;

namespace bookstore_Management.Models
{
    public class BookAuthor
    {
        [Required]
        [StringLength(6)]
        [Column("book_id")]
        public string BookId { get; set; }

        [Required]
        [StringLength(6)]
        [Column("author_id")]
        public string AuthorId { get; set; }
        
        [Required]
        [Column("role")] // Vai trò: Tác giả chính, Đồng tác giả, Dịch giả, Hiệu đính
        public AuthorRole Role { get; set; } =  AuthorRole.MainAuthor;

        // Navigation properties
        public virtual Book Book { get; set; }
        public virtual Author Author { get; set; }
    }
}