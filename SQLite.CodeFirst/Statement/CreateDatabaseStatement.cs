using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SQLite.CodeFirst.Statement
{
    internal class CreateDatabaseStatement : Collection<CreateTableStatement>, IStatement
    {
        private const string CreateTableStatementSeperator = "\r\n";

        public CreateDatabaseStatement() { }

        public CreateDatabaseStatement(IEnumerable<CreateTableStatement> createTableStatements)
        {
            foreach (var createTableStatement in createTableStatements)
            {
                Add(createTableStatement);
            }
        }

        public string CreateStatement()
        {
            return String.Join(CreateTableStatementSeperator, this.Select(c => c.CreateStatement()));
        }
    }
}
