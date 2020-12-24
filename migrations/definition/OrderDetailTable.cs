using System.Collections.Generic;

namespace Readible.Migrations
{
    public class OrderDetailTable : DbTable
    {
        public OrderDetailTable()
        {
            TableName = MigrationConst.TABLE_ORDER_DETAIL;
            PrimaryColumn = MigrationConst.COLUMN_ID;
            TableColumns = new List<DbTableColumn>();

            Add(new DbTableColumn("id", "serial").PrimaryKey());
            Add(new DbTableColumn("order_id", "integer").NotNull());
            Add(new DbTableColumn("book_isbn", "char(13)").NotNull());
            Add(new DbTableColumn("amount", "integer").Default(0).NotNull());
            Add(new DbTableColumn("price", "double precision").Default(0.0).NotNull());
        }
    }
}