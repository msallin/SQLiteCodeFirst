using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using SQLiteEfCodeFirstDbCreator.Statement;

namespace SQLiteEfCodeFirstDbCreator.Builder
{
    internal class ForeignKeyCollectionBuilder : IStatementBuilder<ColumnCollection>
    {
        private readonly IEnumerable<AssociationType> associationTypes;

        public ForeignKeyCollectionBuilder(IEnumerable<AssociationType> associationTypes)
        {
            this.associationTypes = associationTypes;
        }

        public ColumnCollection BuildStatement()
        {
            var columnDefStatement = new ColumnCollection
            {
                ColumnStatements = GetForeignKeyStatements().ToList()
            };

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
