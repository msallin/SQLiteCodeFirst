using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using SQLite.CodeFirst.Builder.NameCreators;
using SQLite.CodeFirst.Statement;
using SQLite.CodeFirst.Utility;
using System.Linq;
using SQLite.CodeFirst.Extensions;

namespace SQLite.CodeFirst.Builder
{
    internal class CreateTableStatementBuilder : IStatementBuilder<CreateTableStatement>
    {
        private readonly EntitySet entitySet;
        private readonly AssociationTypeContainer associationTypeContainer;

        public CreateTableStatementBuilder(EntitySet entitySet, AssociationTypeContainer associationTypeContainer)
        {
            this.entitySet = entitySet;
            this.associationTypeContainer = associationTypeContainer;
        }

        public CreateTableStatement BuildStatement()
        {
            var nonAutoincrementKeys = entitySet.ElementType.KeyMembers
                .Where(km => (km as EdmProperty)?.GetCustomAnnotation<AutoincrementAttribute>() == null);

            var simpleColumnCollection = new ColumnStatementCollectionBuilder(entitySet.ElementType.Properties).BuildStatement();
            var foreignKeyCollection = new ForeignKeyStatementBuilder(associationTypeContainer.GetAssociationTypes(entitySet.Name)).BuildStatement();

            var columnStatements = new List<IStatement>();
            columnStatements.AddRange(simpleColumnCollection);
            if (nonAutoincrementKeys.Any())
            {
                var primaryKeyStatement = new PrimaryKeyStatementBuilder(nonAutoincrementKeys).BuildStatement();
                columnStatements.Add(primaryKeyStatement);
            }
            columnStatements.AddRange(foreignKeyCollection);

            return new CreateTableStatement
            {
                TableName = TableNameCreator.CreateTableName(entitySet.Table),
                ColumnStatementCollection = new ColumnStatementCollection(columnStatements)
            };
        }
    }
}
