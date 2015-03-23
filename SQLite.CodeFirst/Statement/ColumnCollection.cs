using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLite.CodeFirst.Statement
{
    internal class ColumnCollection : IStatement
    {
        private const string ColumnStatementSeperator = ", ";

        public IEnumerable<IStatement> ColumnStatements { get; set; }

        public string CreateStatement()
        {
            return String.Join(ColumnStatementSeperator, ColumnStatements.Select(c => c.CreateStatement()));
        }
    }
}
