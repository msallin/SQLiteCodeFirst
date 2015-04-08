using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using SQLite.CodeFirst.Extensions;
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
            var simpleColumnCollection = new ColumnStatementCollectionBuilder(entityType.Properties).BuildStatement();
            var primaryKeyStatement = new PrimaryKeyStatementBuilder(entityType.KeyMembers).BuildStatement();
            var foreignKeyCollection = new ForeignKeyStatementBuilder(associationTypes).BuildStatement();

            var columnStatements = new List<IStatement>();
            columnStatements.AddRange(simpleColumnCollection);
            columnStatements.Add(primaryKeyStatement);
            columnStatements.AddRange(foreignKeyCollection);

            return new CreateTableStatement
            {
                TableName = entityType.GetTableName(),
                ColumnStatementCollection = new ColumnStatementCollection(columnStatements)
            };
        }
    }
}
