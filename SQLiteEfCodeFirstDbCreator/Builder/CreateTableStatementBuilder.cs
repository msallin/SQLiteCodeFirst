using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using SQLiteEfCodeFirstDbCreator.Statement;

namespace SQLiteEfCodeFirstDbCreator.Builder
{
    internal class CreateTableStatementBuilder : IStatementBuilder<CreateTableStatement>
    {
        private readonly EntityType entityType;
        private readonly IEnumerable<AssociationType> associationTypes;

        public CreateTableStatementBuilder(EntityType entityType, IEnumerable<AssociationType> associationTypes)
        {
            this.entityType = entityType;
            this.associationTypes = associationTypes;
        }

        public CreateTableStatement BuildStatement()
        {
            var simpleColumnCollection = new ColumnCollectionBuilder(entityType.Properties).BuildStatement();
            var foreignKeyCollection = new ForeignKeyCollectionBuilder(associationTypes).BuildStatement();
            var columnCollection = new ColumnCollection
            {
                ColumnStatements = simpleColumnCollection.ColumnStatements.Concat(foreignKeyCollection.ColumnStatements)
            };

            return new CreateTableStatement
            {
                TableName = entityType.Name,
                ColumnCollection = columnCollection
            };
        }
    }
}
