using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Readible.Models
{
    [Table("orders")]
    public class Order
    {
        [Key] [Column("id")] public int Id { get; set; }

        [Required] [Column("customer_id")] public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        [Required] [Column("status_id")] public int StatusId { get; set; }

        public virtual OrderStatus Status { get; set; }

        [Column("contact")] public string Contact { get; set; }

        [Column("address")] public string Address { get; set; }

        [Column("phone")] public string Phone { get; set; }

        [Column("note")] public string Note { get; set; }

        [Required] [Column("total_price")] public double TotalPrice { get; set; }

        [Required] [Column("total_items")] public int TotalItem { get; set; }

        [Column("created_at")] public DateTime? CreatedAt { get; set; }

        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        [Column("confirmer_id")] public int? ConfirmerId { get; set; }
		
		[Column("confirmed_at")] public DateTime? ConfirmedAt { get; set; }

        public virtual Manager ConfirmedManager { get; set; }

        [Column("completer_id")] public int? CompleterId { get; set; }
		
		[Column("completed_at")] public DateTime? CompletedAt { get; set; }

        public virtual Manager CompletedManager { get; set; }
    }
}