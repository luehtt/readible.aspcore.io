using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Readible.Models
{
    [Table("book_categories")]
    public class BookCategory
    {
        [Key] [Column("id")] public int Id { get; set; }

        [Required] [Column("name")] public string Name { get; set; }

        [Column("created_at")] public DateTime? CreatedAt { get; set; }

        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }
    }
}