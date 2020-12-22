using System.Collections.Generic;
using static Readible.Migrations.MigrationConst;

namespace Readible.Migrations
{
    public class UserTable : DbTable
    {
        public UserTable()
        {
            TableName = TABLE_USER;
            PrimaryColumn = COLUMN_ID;
            IndexColumn = COLUMN_USERNAME;
            TableColumns = new List<DbTableColumn>();

            Add(new DbTableColumn("id", "serial").PrimaryKey());
            Add(new DbTableColumn("username", "varchar(32)").Unique().NotNull());
            Add(new DbTableColumn("email", "varchar(255)").Unique().NotNull());
            Add(new DbTableColumn("password", "varchar(255)").NotNull());
            Add(new DbTableColumn("active", "boolean").Default("true").NotNull());
            Add(new DbTableColumn("user_role_id", "integer").NotNull());
            AddTimestamp();
        }
    }
}