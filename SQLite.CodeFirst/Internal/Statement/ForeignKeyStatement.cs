using SQLite.CodeFirst.Builder.NameCreators;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLite.CodeFirst.Statement
{
    internal class ForeignKeyStatement : IStatement
    {
        private const string Template = "FOREIGN KEY ({foreign-key}) REFERENCES {referenced-table}({referenced-id})";
        private const string CascadeDeleteStatement = "ON DELETE CASCADE";

        public IEnumerable<string> ForeignKey { get; set; }
        public string ForeignTable { get; set; }
        public IEnumerable<string> ForeignPrimaryKey { get; set; }
        public bool CascadeDelete { get; set; }

        public string CreateStatement()
        {
            var sb = new StringBuilder(Template);
            sb.Replace("{foreign-key}", string.Join(", ", ForeignKey.Select(c => ColumnNameCreator.EscapeName(c))));
            sb.Replace("{referenced-table}", ForeignTable);
            sb.Replace("{referenced-id}", string.Join(", ", ForeignPrimaryKey.Select(c => ColumnNameCreator.EscapeName(c))));
            if (CascadeDelete)
            {
                sb.Append(" " + CascadeDeleteStatement);
            }

            return sb.ToString();
        }
    }
}
