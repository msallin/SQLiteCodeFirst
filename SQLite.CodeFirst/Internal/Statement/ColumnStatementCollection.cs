using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SQLite.CodeFirst.Statement
{
    internal class ColumnStatementCollection : Collection<IStatement>, IStatementCollection
    {
        private const string ColumnStatementSeperator = ", ";

        public ColumnStatementCollection() { }

        public ColumnStatementCollection(IEnumerable<IStatement> statements)
        {
            foreach (var statement in statements)
            {
                Add(statement);
            }
        }

        public string CreateStatement()
        {
            return String.Join(ColumnStatementSeperator, this.Select(c => c.CreateStatement()));
        }
    }
}
