using System.Collections.Generic;
using static Readible.Migrations.MigrationConst;

namespace Readible.Migrations
{
    public class BookCommentTable : DbTable
    {
        public BookCommentTable()
        {
            TableName = TABLE_BOOK_COMMENT;
            PrimaryColumn = COLUMN_ID;
            TableColumns = new List<DbTableColumn>();

            Add(new DbTableColumn("id", "serial").PrimaryKey());
            Add(new DbTableColumn("book_isbn", "char(13)").NotNull());
            Add(new DbTableColumn("customer_id", "integer").NotNull());
            Add(new DbTableColumn("rating", "integer").Default(0).NotNull());
            Add(new DbTableColumn("comment", "text"));
            AddTimestamp();
        }
    }
}