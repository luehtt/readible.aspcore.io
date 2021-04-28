using System.Collections.Generic;
using System.Globalization;
using Readible.Models;

namespace Readible.Migrations
{
    public class OrderStatusTable : DbTable
    {
        public OrderStatusTable()
        {
            TableName = MigrationConst.TABLE_ORDER_STATUS;
            PrimaryColumn = MigrationConst.COLUMN_ID;
            TableColumns = new List<DbTableColumn>();

            Add(new DbTableColumn("id", "integer").PrimaryKey());
            Add(new DbTableColumn("name", "varchar(32)").NotNull().Unique());
            Add(new DbTableColumn("locale", "varchar(32)").NotNull().Unique());
        }

        public static IEnumerable<OrderStatus> Seed()
        {
            var data = new[] {"PENDING", "DELIVERING", "SUCCESS", "FAILED"};
            var length = data.Length;
            var list = new List<OrderStatus>(length);

            for (var i = 0; i < length; i++)
                list.Add(new OrderStatus(i + 1, data[i],
                    CultureInfo.CurrentCulture.TextInfo.ToTitleCase(data[i].ToLower())));

            return list;
        }
    }
}