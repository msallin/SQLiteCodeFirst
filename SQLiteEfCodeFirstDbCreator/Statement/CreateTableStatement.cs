using System.Text;

namespace SQLiteEfCodeFirstDbCreator.Statement
{
    internal class CreateTableStatement : IStatement
    {
        private const string Template = "CREATE TABLE {table-name} ({column-def});";

        public string TableName { get; set; }
        public ColumnCollection ColumnCollection { get; set; }

        public string CreateStatement()
        {
            var sb = new StringBuilder(Template);

            sb.Replace("{table-name}", TableName);
            sb.Replace("{column-def}", ColumnCollection.CreateStatement());

            return sb.ToString();
        }
    }
}
