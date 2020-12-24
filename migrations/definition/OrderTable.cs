using System.Collections.Generic;
using static Readible.Migrations.MigrationConst;

namespace Readible.Migrations
{
    public class OrderTable : DbTable
    {
        public OrderTable()
        {
            TableName = TABLE_ORDER;
            PrimaryColumn = COLUMN_ID;
            TableColumns = new List<DbTableColumn>();

            Add(new DbTableColumn("id", "serial").PrimaryKey());
            Add(new DbTableColumn("customer_id", "integer").NotNull());
            Add(new DbTableColumn("status_id", "integer").NotNull());
            Add(new DbTableColumn("total_items", "integer").Default(0).NotNull());
            Add(new DbTableColumn("total_price", "double precision").Default(0.0).NotNull());
            Add(new DbTableColumn("contact", "varchar(80)").NotNull());
            Add(new DbTableColumn("address", "varchar(255)").NotNull());
            Add(new DbTableColumn("phone", "varchar(16)").NotNull());
            Add(new DbTableColumn("note", "text"));
            Add(new DbTableColumn("confirmer_id", "integer"));
            Add(new DbTableColumn("completer_id", "integer"));
			Add(new DbTableColumn("confirmed_at", "timestamp"));
            Add(new DbTableColumn("completed_at", "timestamp"));
            AddTimestamp();
        }

    }
}
