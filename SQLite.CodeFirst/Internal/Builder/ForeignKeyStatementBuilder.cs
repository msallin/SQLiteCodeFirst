using System.Collections.Generic;
using System.Linq;
using SQLite.CodeFirst.Statement;
using SQLite.CodeFirst.Utility;

namespace SQLite.CodeFirst.Builder
{
    internal class ForeignKeyStatementBuilder : IStatementBuilder<ColumnStatementCollection>
    {
        private readonly IEnumerable<SqliteAssociationType> associationTypes;

        public ForeignKeyStatementBuilder(IEnumerable<SqliteAssociationType> associationTypes)
        {
            this.associationTypes = associationTypes;
        }

        public ColumnStatementCollection BuildStatement()
        {
            var columnDefStatement = new ColumnStatementCollection(GetForeignKeyStatements().ToList());
            return columnDefStatement;
        }

        private IEnumerable<ForeignKeyStatement> GetForeignKeyStatements()
        {
            foreach (var associationType in associationTypes)
            {
                yield return new ForeignKeyStatement
                {
                    ForeignKey = associationType.ForeignKey,
                    ForeignTable = associationType.FromTableName,
                    ForeignPrimaryKey = associationType.ForeignPrimaryKey,
                    CascadeDelete = associationType.CascadeDelete
                };
            }
        }
    }
}
