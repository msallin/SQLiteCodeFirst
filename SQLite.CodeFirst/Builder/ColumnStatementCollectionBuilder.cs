using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using SQLite.CodeFirst.Statement;
using SQLite.CodeFirst.Statement.ColumnConstraint;

namespace SQLite.CodeFirst.Builder
{
    internal class ColumnStatementCollectionBuilder : IStatementBuilder<ColumnStatementCollection>
    {
        private readonly IEnumerable<EdmProperty> properties;

        public ColumnStatementCollectionBuilder(IEnumerable<EdmProperty> properties)
        {
            this.properties = properties;
        }

        public ColumnStatementCollection BuildStatement()
        {
            var columnDefStatement = new ColumnStatementCollection(CreateColumnStatements().ToList());
            return columnDefStatement;
        }

        private IEnumerable<ColumnStatement> CreateColumnStatements()
        {
            foreach (var property in properties)
            {
                var columnStatement = new ColumnStatement
                {
                    ColumnName = property.Name,
                    TypeName = property.TypeName,
                    ColumnConstraints = new ColumnConstraintCollection()
                };

                AdjustDatatypeForAutogenerationIfNecessary(property, columnStatement);
                AddNullConstraintIfNecessary(property, columnStatement);

                yield return columnStatement;
            }
        }

        private static void AdjustDatatypeForAutogenerationIfNecessary(EdmProperty property, ColumnStatement columnStatement)
        {
            if (property.StoreGeneratedPattern == StoreGeneratedPattern.Identity)
            {
                // Must be INTEGER else SQLite will not generate the Ids
                columnStatement.TypeName = columnStatement.TypeName.ToLower() == "int" ? "INTEGER" : columnStatement.TypeName;
            }
        }

        private static void AddNullConstraintIfNecessary(EdmProperty property, ColumnStatement columnStatement)
        {
            if (!property.Nullable && property.StoreGeneratedPattern != StoreGeneratedPattern.Identity)
            {
                // Only mark it as NotNull if it should not be generated.
                columnStatement.ColumnConstraints.Add(new NotNullConstraint());
            }
        }
    }
}
