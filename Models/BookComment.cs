using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Readible.Models
{
    [Table("book_comments")]
    public class BookComment
    {
        [Key] [Column("id")] public int Id { get; set; }

        [Required] [Column("book_isbn")] public string BookIsbn { get; set; }

        public virtual Book Book { get; set; }

        [Required] [Column("customer_id")] public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        [Required] [Column("rating")] public int Rating { get; set; }

        [Column("comment")] public string Comment { get; set; }

        [Column("created_at")] public DateTime? CreatedAt { get; set; }

        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }
    }
}