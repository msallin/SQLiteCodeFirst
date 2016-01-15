using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SQLite.CodeFirst.Statement
{
    internal class CreateIndexStatementCollection : Collection<IStatement>, IStatementCollection
    {
        private const string StatementSeperator = "\r\n";

        public CreateIndexStatementCollection(IEnumerable<IStatement> createIndexStatements)
        {
            foreach (var createIndexStatement in createIndexStatements)
            {
                Add(createIndexStatement);
            }
        }

        public string CreateStatement()
        {
            return String.Join(StatementSeperator, this.Select(e => e.CreateStatement()));
        }
    }
}
