using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using SQLite.CodeFirst.Builder.NameCreators;
using SQLite.CodeFirst.Statement;
using SQLite.CodeFirst.Utility;
using SQLite.CodeFirst.Extensions;
using System.Linq;

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
            var keyMembers = entitySet.ElementType.KeyMembers.Cast<EdmProperty>().ToArray();

            // Only create a CompositePrimaryKeyStatement if there is a composite primary key.
            // If there is just one key member this is handled using a constraint.
            CompositePrimaryKeyStatement compositePrimaryKeyStatement = null;
            if (keyMembers.Length > 1)
            {
                compositePrimaryKeyStatement = new CompositePrimaryKeyStatementBuilder(keyMembers).BuildStatement();
            }

            var simpleColumnCollection = new ColumnStatementCollectionBuilder(entitySet.ElementType.Properties, keyMembers).BuildStatement();
            var foreignKeyCollection = new ForeignKeyStatementBuilder(associationTypeContainer.GetAssociationTypes(entitySet.Name)).BuildStatement();

            var columnStatements = new List<IStatement>();
            columnStatements.AddRange(simpleColumnCollection);
            columnStatements.AddIfNotNull(compositePrimaryKeyStatement);
            columnStatements.AddRange(foreignKeyCollection);

            return new CreateTableStatement
            {
                TableName = NameCreator.EscapeName(entitySet.Table),
                ColumnStatementCollection = new ColumnStatementCollection(columnStatements)
            };
        }
    }
}
