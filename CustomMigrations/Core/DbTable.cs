using System.Collections.Generic;
using System.Text;

namespace Readible.Migrations
{
    public abstract class DbTable
    {
        public string TableName { get; set; }
        public string PrimaryColumn { get; set; }
        public string IndexColumn { get; set; }
        public List<DbTableColumn> TableColumns { get; set; }

        public void Add(DbTableColumn tableColumn)
        {
            TableColumns.Add(tableColumn);
        }

        public void AddTimestamp()
        {
            TableColumns.Add(new DbTableColumn("created_at", "timestamp"));
            TableColumns.Add(new DbTableColumn("updated_at", "timestamp"));
        }

        public string MakeCreateCmd()
        {
            var stringBuilder = new StringBuilder();
            var column = TableColumns.Count;
            for (var i = 0; i < column; i++) stringBuilder.Append(TableColumns[i].MakeCmd + ", ");
            stringBuilder.Length -= 2;
            return $"CREATE TABLE {TableName} ({stringBuilder})";
        }

        public string MakeDropCmd()
        {
            return $"DROP TABLE IF EXISTS {TableName}";
        }

        public string MakeDeleteDataCmd()
        {
            return $"DELETE FROM {TableName}";
        }

        public string MakeSetSequenceCmd(int number)
        {
            return $"ALTER SEQUENCE {TableName}_{PrimaryColumn}_seq RESTART WITH {number}";
        }

        public string MakeIndexCmd()
        {
            return $"CREATE INDEX idx_{TableName}_{IndexColumn} ON {TableName}({IndexColumn})";
        }

        public string MakeGetRandomCmd(int amount)
        {
            return $"SELECT * FROM {TableName} ORDER BY random() LIMIT {amount}";
        }
    }
}