using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bookstore_Management.Models
{
    public class Author
    {
        [Required]
        [StringLength(6)]
        [Column("id")]
        public string AuthorId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("name")]
        public string Name { get; set; }
        
        [Required]
        [StringLength(255)]
        [Column("bio")]
        public string Bio { get; set; }
        
        // navigation properties
        public virtual ICollection<BookAuthor> BookAuthors { get; set; }
    }
}