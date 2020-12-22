using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Readible.Models
{
    [Table("customers")]
    public class Customer
    {
        [Key] [Column("user_id")] public int UserId { get; set; }

        [Required] [Column("fullname")] public string Fullname { get; set; }

        [Column("birth")] public int Birth { get; set; }

        [Column("male")] public bool Male { get; set; }

        [Column("address")] public string Address { get; set; }

        [Column("phone")] public string Phone { get; set; }

        [Column("image")] public string Image { get; set; }

        [Column("created_at")] public DateTime? CreatedAt { get; set; }

        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<BookComment> BookComments { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}