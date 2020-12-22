using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Readible.Models
{
    [Table("user_roles")]
    public class UserRole
    {
        public UserRole(int id, string name, string locale)
        {
            Id = id;
            Name = name;
            Locale = locale;
        }

        [Key] [Column("id")] public int Id { get; set; }

        [Required] [Column("name")] public string Name { get; set; }

        [Column("locale")] public string Locale { get; set; }
    }
}