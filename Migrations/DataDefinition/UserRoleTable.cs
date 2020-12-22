using System.Collections.Generic;
using static Readible.Migrations.MigrationConst;

namespace Readible.Migrations
{
    public class UserRoleTable : DbTable
    {
        public UserRoleTable()
        {
            TableName = TABLE_USER_ROLE;
            PrimaryColumn = COLUMN_ID;
            TableColumns = new List<DbTableColumn>();

            Add(new DbTableColumn("id", "integer").PrimaryKey());
            Add(new DbTableColumn("name", "varchar(32)").NotNull().Unique());
            Add(new DbTableColumn("locale", "varchar(32)").NotNull().Unique());
        }
    }
}