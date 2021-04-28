using Readible.Models;
using System.Collections.Generic;
using System.Globalization;

namespace Readible.Migrations.DataSeeders
{
    public class MigrationTableSeeder
    {
        public List<UserRole> UserRoles { get; set; }
        public List<OrderStatus> OrderStatuses { get; set; }

        public MigrationTableSeeder()
        {
            UserRoles = SeedUserRoles();
            OrderStatuses = SeedOrderStatuses();
        }

        private List<UserRole> SeedUserRoles()
        {
            var data = new[] { "ADMIN", "MANAGER", "CUSTOMER", "ETC" };
            var count = data.Length;
            var list = new List<UserRole>();

            for (var i = 0; i < count; i++) list.Add(new UserRole(i + 1, data[i], CultureInfo.CurrentCulture.TextInfo.ToTitleCase(data[i].ToLower())));
            return list;
        }

        private List<OrderStatus> SeedOrderStatuses()
        {
            var data = new[] { "PENDING", "DELIVERING", "SUCCESS", "FAILED" };
            var count = data.Length;
            var list = new List<OrderStatus>();

            for (var i = 0; i < count; i++) list.Add(new OrderStatus(i + 1, data[i], CultureInfo.CurrentCulture.TextInfo.ToTitleCase(data[i].ToLower())));
            return list;
        }
    }
}
