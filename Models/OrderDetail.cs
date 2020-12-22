using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Readible.Models
{
    [Table("order_details")]
    public class OrderDetail
    {
        [Key] [Column("id")] public int Id { get; set; }

        [Required] [Column("order_id")] public int OrderId { get; set; }

        public virtual Order Order { get; set; }

        [Required] [Column("book_isbn")] public string BookIsbn { get; set; }

        public virtual Book Book { get; set; }

        [Required] [Column("amount")] public int Amount { get; set; }

        [Required] [Column("price")] public double Price { get; set; }
    }
}