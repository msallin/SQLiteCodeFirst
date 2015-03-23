using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using SQLite.CodeFirst.Statement;
using SQLite.CodeFirst.Statement.ColumnConstraint;

namespace SQLite.CodeFirst.Builder
{
    internal class ColumnCollectionBuilder : IStatementBuilder<ColumnCollection>
    {
        private readonly IEnumerable<EdmProperty> properties;

        public ColumnCollectionBuilder(IEnumerable<EdmProperty> properties)
        {
            this.properties = properties;
        }

        public ColumnCollection BuildStatement()
        {
            var columnDefStatement = new ColumnCollection
            {
                ColumnStatements = CreateColumnStatements().ToList()
            };

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
                    ColumnConstraints = new ColumnConstraintCollection
                    {
                        ColumnConstraints = new List<IColumnConstraint>()
                    }
                };

                if (!property.Nullable && property.StoreGeneratedPattern != StoreGeneratedPattern.Identity) // Only mark it as NotNull if it should not be generated.
                    columnStatement.ColumnConstraints.ColumnConstraints.Add(new NotNullConstraint());

                if (property.StoreGeneratedPattern == StoreGeneratedPattern.Identity)
                {
                    columnStatement.ColumnConstraints.ColumnConstraints.Add(new PrimaryKeyConstraint());
                    // Must be INTEGER else SQLite will not generate the Ids
                    columnStatement.TypeName = columnStatement.TypeName.ToLower() == "int" ? "INTEGER" : columnStatement.TypeName; 
                }

                yield return columnStatement;
            }
        }
    }
}
