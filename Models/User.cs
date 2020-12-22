using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Readible.Models
{
    [Table("users")]
    public class User
    {
        [Key] [Column("id")] public int Id { get; set; }

        [Required] [Column("username")] public string Username { get; set; }

        [Required] [Column("email")] public string Email { get; set; }

        [JsonIgnore]
        [Required]
        [Column("password")]
        public string Password { get; set; }

        [Required] [Column("active")] public bool Active { get; set; }

        [Required] [Column("user_role_id")] public int UserRoleId { get; set; }

        [Column("created_at")] public DateTime? CreatedAt { get; set; }

        [Column("updated_at")] public DateTime? UpdatedAt { get; set; }

        public virtual UserRole UserRole { get; set; }
    }
}