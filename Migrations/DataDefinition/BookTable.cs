using System.Collections.Generic;

namespace Readible.Migrations
{
    public class BookTable : DbTable
    {
        public BookTable()
        {
            TableName = MigrationConst.TABLE_BOOK;
            PrimaryColumn = MigrationConst.COLUMN_ISBN;
            TableColumns = new List<DbTableColumn>();

            Add(new DbTableColumn("isbn", "char(13)").PrimaryKey());
            Add(new DbTableColumn("title", "varchar(255)").NotNull());
            Add(new DbTableColumn("author", "varchar(255)").NotNull());
            Add(new DbTableColumn("publisher", "varchar(255)").NotNull());
            Add(new DbTableColumn("published", "date").NotNull());
            Add(new DbTableColumn("language", "varchar(32)").NotNull());
            Add(new DbTableColumn("category_id", "integer").NotNull());
            Add(new DbTableColumn("info", "text"));
            Add(new DbTableColumn("image", "text"));
            Add(new DbTableColumn("page", "integer").Default(0).NotNull());
            Add(new DbTableColumn("price", "double precision").Default(0.0).NotNull());
            Add(new DbTableColumn("discount", "integer").Default(0).NotNull());
            Add(new DbTableColumn("viewed", "integer").Default(0).NotNull());
            Add(new DbTableColumn("active", "boolean").Default(true).NotNull());
            AddTimestamp();
        }
    }
}