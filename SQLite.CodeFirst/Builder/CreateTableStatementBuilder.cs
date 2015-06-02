using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using SQLite.CodeFirst.Extensions;
using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst.Builder
{
    internal class CreateTableStatementBuilder : IStatementBuilder<CreateTableStatement>
    {
        private readonly EntitySet entitySet;
        private readonly IEnumerable<AssociationType> associationTypes;

        public CreateTableStatementBuilder(EntitySet entitySet, IEnumerable<AssociationType> associationTypes)
        {
            this.entitySet = entitySet;
            this.associationTypes = associationTypes;
        }

        public CreateTableStatement BuildStatement()
        {
            var simpleColumnCollection = new ColumnStatementCollectionBuilder(entitySet.ElementType.Properties).BuildStatement();
            var primaryKeyStatement = new PrimaryKeyStatementBuilder(entitySet.ElementType.KeyMembers).BuildStatement();
            var foreignKeyCollection = new ForeignKeyStatementBuilder(associationTypes).BuildStatement();

            var columnStatements = new List<IStatement>();
            columnStatements.AddRange(simpleColumnCollection);
            columnStatements.Add(primaryKeyStatement);
            columnStatements.AddRange(foreignKeyCollection);

            return new CreateTableStatement
            {
                TableName = entitySet.Table,
                ColumnStatementCollection = new ColumnStatementCollection(columnStatements)
            };
        }
    }
}
