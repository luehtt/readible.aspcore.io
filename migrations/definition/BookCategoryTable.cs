using System.Collections.Generic;
using static Readible.Migrations.MigrationConst;

namespace Readible.Migrations
{
    public class BookCategoryTable : DbTable
    {
        public BookCategoryTable()
        {
            TableName = TABLE_BOOK_CATEGORY;
            PrimaryColumn = COLUMN_ID;
            TableColumns = new List<DbTableColumn>();

            Add(new DbTableColumn("id", "serial").PrimaryKey());
            Add(new DbTableColumn("name", "varchar(32)").Unique().NotNull());
            AddTimestamp();
        }
    }
}