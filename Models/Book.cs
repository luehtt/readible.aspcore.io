using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Readible.Models
{
    [Table("books")]
    public class Book
    {
        [Key] [Column("isbn")] public string Isbn { get; set; }

        [Required] [Column("title")] public string Title { get; set; }

        [Required] [Column("author")] public string Author { get; set; }

        [Required] [Column("publisher")] public string Publisher { get; set; }

        [Column("published")] public DateTime? Published { get; set; }

        [Column("language")] public string Language { get; set; }

        [Column("page")] public int Page { get; set; }

        [Required] [Column("price")] public double Price { get; set; }

        [Required] [Column("active")] public bool Active { get; set; }

        [Column("discount")] public int Discount { get; set; }

        [Column("viewed")] public int Viewed { get; set; }

        [Column("image")] public string Image { get; set; }

        [Column("info")] public string Info { get; set; }

        [Required] [Column("category_id")] public int CategoryId { get; set; }

        public virtual BookCategory Category { get; set; }

        [Column("created_at")] public DateTime? CreatedAt { get; set; }

        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<BookComment> BookComments { get; set; }
    }
}