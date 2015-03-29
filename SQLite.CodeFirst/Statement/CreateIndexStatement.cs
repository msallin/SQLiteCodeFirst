using System;
using System.Collections.Generic;
using System.Text;

namespace SQLite.CodeFirst.Statement
{
    internal class CreateIndexStatement : IStatement
    {
        private const string Template = "CREATE {unique} INDEX {index-name} ON {table-name} ({column-def});";

        private const string ColumnNameSeperator = ", ";

        public string Name { get; set; }
        public string Table { get; set; }
        public IEnumerable<string> ColumnNames { get; set; }
        public bool IsUnique { get; set; }

        public string CreateStatement()
        {
            var stringBuilder = new StringBuilder(Template);

            stringBuilder.Replace("{unique}", IsUnique ? "UNIQUE" : string.Empty);
            stringBuilder.Replace("{index-name}", Name);
            stringBuilder.Replace("{table-name}", Table);

            string columnNames = String.Join(ColumnNameSeperator, ColumnNames);
            stringBuilder.Replace("{column-def}", columnNames);

            return stringBuilder.ToString();
        }
    }
}
