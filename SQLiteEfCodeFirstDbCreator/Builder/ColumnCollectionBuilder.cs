using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using SQLiteEfCodeFirstDbCreator.Statement;
using SQLiteEfCodeFirstDbCreator.Statement.ColumnConstraint;

namespace SQLiteEfCodeFirstDbCreator.Builder
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

                if (!property.Nullable)
                    columnStatement.ColumnConstraints.ColumnConstraints.Add(new NotNullConstraint());

                if(property.StoreGeneratedPattern == StoreGeneratedPattern.Identity)
                    columnStatement.ColumnConstraints.ColumnConstraints.Add(new PrimaryKeyConstraint());

                yield return columnStatement;
            }
        }
    }
}
