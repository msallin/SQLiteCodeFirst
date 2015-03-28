using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst.Builder
{
    internal class ForeignKeyStatementBuilder : IStatementBuilder<ColumnStatementCollection>
    {
        private readonly IEnumerable<AssociationType> associationTypes;

        public ForeignKeyStatementBuilder(IEnumerable<AssociationType> associationTypes)
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
                    ForeignKey = associationType.Constraint.ToProperties.Select(x => x.Name),
                    ForeignTable = associationType.Constraint.FromRole.Name,
                    ForeignPrimaryKey = associationType.Constraint.FromProperties.Select(x => x.Name),
                    CascadeDelete = associationType.Constraint.FromRole.DeleteBehavior == OperationAction.Cascade
                };
            }
        }
    }
}
