namespace Readible.Migrations
{
    public class DbTableColumn
    {
        public DbTableColumn(string columnName, string dataType)
        {
            MakeCmd = $"{columnName} {dataType}";
        }

        public string MakeCmd { get; private set; }

        public DbTableColumn PrimaryKey()
        {
            MakeCmd = MakeCmd + " not null primary key";
            return this;
        }

        public DbTableColumn NotNull()
        {
            MakeCmd = MakeCmd + " not null";
            return this;
        }

        public DbTableColumn Default(object defaultData)
        {
            MakeCmd = MakeCmd + $" default {defaultData}";
            return this;
        }

        public DbTableColumn Unique()
        {
            MakeCmd = MakeCmd + " unique";
            return this;
        }
    }
}