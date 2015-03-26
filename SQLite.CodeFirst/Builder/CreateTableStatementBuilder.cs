using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst.Builder
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
                TableName = GetTableName(),
                ColumnCollection = columnCollection
            };
        }

        private string GetTableName()
        {
            MetadataProperty metadataProperty;
            if (entityType.MetadataProperties.TryGetValue("TableName", false, out metadataProperty))
            {
                return metadataProperty.Value.ToString();
            }

            return entityType.Name;
        }
    }
}
