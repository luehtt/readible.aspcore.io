using System.Collections.Generic;
using static Readible.Migrations.MigrationConst;

namespace Readible.Migrations
{
    public class CustomerTable : DbTable
    {
        public CustomerTable()
        {
            TableName = TABLE_CUSTOMER;
            PrimaryColumn = COLUMN_USER_ID;
            TableColumns = new List<DbTableColumn>();

            Add(new DbTableColumn("user_id", "integer").PrimaryKey());
            Add(new DbTableColumn("fullname", "varchar(255)").NotNull());
            Add(new DbTableColumn("birth", "integer").NotNull());
            Add(new DbTableColumn("male", "boolean").Default(true).NotNull());
            Add(new DbTableColumn("address", "varchar(255)"));
            Add(new DbTableColumn("phone", "varchar(16)"));
            Add(new DbTableColumn("image", "text"));
            AddTimestamp();
        }
    }
}