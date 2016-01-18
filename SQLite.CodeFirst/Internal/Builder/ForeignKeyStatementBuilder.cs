using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using SQLite.CodeFirst.Statement;
using SQLite.CodeFirst.Utility;

namespace SQLite.CodeFirst.Builder
{
    internal class ForeignKeyStatementBuilder : IStatementBuilder<ColumnStatementCollection>
    {
        private readonly IEnumerable<AssociationTypeWrapper> associationTypes;

        public ForeignKeyStatementBuilder(IEnumerable<AssociationTypeWrapper> associationTypes)
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
                    ForeignKey = associationType.AssociationType.Constraint.ToProperties.Select(x => x.Name),
                    ForeignTable = associationType.FromTableName,
                    ForeignPrimaryKey = associationType.AssociationType.Constraint.FromProperties.Select(x => x.Name),
                    CascadeDelete = associationType.AssociationType.Constraint.FromRole.DeleteBehavior == OperationAction.Cascade
                };
            }
        }
    }
}
