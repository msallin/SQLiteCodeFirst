using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite.CodeFirst.Builder.NameCreators;

namespace SQLite.CodeFirst.Statement
{
    internal class CreateIndexStatement : IStatement
    {
        private const string Template = "CREATE {unique} INDEX {index-name} ON {table-name} ({column-def});";
        private const string ColumnNameSeperator = ", ";

        public string Name { get; set; }
        public string Table { get; set; }
        public ICollection<IndexColumn> Columns { get; set; }
        public bool IsUnique { get; set; }

        public string CreateStatement()
        {
            var stringBuilder = new StringBuilder(Template);

            stringBuilder.Replace("{unique}", IsUnique ? "UNIQUE" : string.Empty);
            stringBuilder.Replace("{index-name}", Name);
            stringBuilder.Replace("{table-name}", Table);

            IEnumerable<string> orderedColumnNames = Columns.OrderBy(c => c.Order).Select(c => c.Name).Select(NameCreator.EscapeName);
            string columnDefinition = String.Join(ColumnNameSeperator, orderedColumnNames);
            stringBuilder.Replace("{column-def}", columnDefinition);

            return stringBuilder.ToString();
        }

        public class IndexColumn
        {
            public int Order { get; set; }
            public string Name { get; set; }
        }
    }
}
