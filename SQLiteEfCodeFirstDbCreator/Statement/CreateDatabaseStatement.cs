using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLiteEfCodeFirstDbCreator.Statement
{
    internal class CreateDatabaseStatement : IStatement
    {
        private const string CreateTableStatementSeperator = "\r\n";

        public IEnumerable<CreateTableStatement> CreateTableStatements { get; set; }

        public string CreateStatement()
        {
            return String.Join(CreateTableStatementSeperator, CreateTableStatements.Select(c => c.CreateStatement()));
        }
    }
}
