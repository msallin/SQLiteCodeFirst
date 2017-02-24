using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using SQLite.CodeFirst.Extensions;
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

                AddMaxLengthConstraintIfNecessary(property, columnStatement);
                AdjustDatatypeForAutogenerationIfNecessary(property, columnStatement);
                AddNullConstraintIfNecessary(property, columnStatement);
                AddUniqueConstraintIfNecessary(property, columnStatement);
                AddCollationConstraintIfNecessary(property, columnStatement);
                AddAutoincrementConstraintIfNecessary(property, columnStatement);

                yield return columnStatement;
            }
        }

        private static void AddMaxLengthConstraintIfNecessary(EdmProperty property, ColumnStatement columnStatement)
        {
            if (property.MaxLength.HasValue)
            {
                columnStatement.ColumnConstraints.Add(new MaxLengthConstraint(property.MaxLength.Value));
            }
        }

        private static void AdjustDatatypeForAutogenerationIfNecessary(EdmProperty property, ColumnStatement columnStatement)
        {
            if (property.StoreGeneratedPattern == StoreGeneratedPattern.Identity)
            {
                // Must be INTEGER else SQLite will not generate the Ids
                columnStatement.TypeName = columnStatement.TypeName.ToUpperInvariant() == "INT" ? "INTEGER" : columnStatement.TypeName;
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

        private static void AddCollationConstraintIfNecessary(EdmProperty property, ColumnStatement columnStatement)
        {
            var value = property.GetCustomAnnotation<CollateAttribute>();
            if (value != null)
            {
                columnStatement.ColumnConstraints.Add(new CollateConstraint { CollationFunction = value.Collation });
            }
        }

        private static void AddUniqueConstraintIfNecessary(EdmProperty property, ColumnStatement columnStatement)
        {
            var value = property.GetCustomAnnotation<UniqueAttribute>();
            if (value != null)
            {
                columnStatement.ColumnConstraints.Add(new UniqueConstraint { OnConflict = value.OnConflict });
            }
        }

        private static void AddAutoincrementConstraintIfNecessary(EdmProperty property, ColumnStatement columnStatement)
        {
            var value = property.GetCustomAnnotation<AutoincrementAttribute>();
            if (value != null)
            {
                columnStatement.ColumnConstraints.Add(new AutoincrementConstraint());
            }
        }
    }
}
