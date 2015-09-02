using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using SQLite.CodeFirst.Statement;
using SQLite.CodeFirst.Utility;

namespace SQLite.CodeFirst.Builder
{
    internal class CreateDatabaseStatementBuilder : IStatementBuilder<CreateDatabaseStatement>
    {
        private readonly EdmModel edmModel;

        public CreateDatabaseStatementBuilder(EdmModel edmModel)
        {
            this.edmModel = edmModel;
        }

        public CreateDatabaseStatement BuildStatement()
        {
            var createTableStatements = GetCreateTableStatements();
            var createIndexStatements = GetCreateIndexStatements();
            var createStatements = createTableStatements.Concat<IStatement>(createIndexStatements);
            var createDatabaseStatement = new CreateDatabaseStatement(createStatements);
            return createDatabaseStatement;
        }

        private IEnumerable<CreateTableStatement> GetCreateTableStatements()
        {
            foreach (var entitySet in edmModel.Container.EntitySets)
            {
                ICollection<AssociationType> associationTypes =
                    edmModel.AssociationTypes.Where(a => a.Constraint.ToRole.Name == entitySet.Name).ToList();

                var b = associationTypes.Select(a => new AssociationTypeWrapper
                {
                    AssociationType = a,
                    FromTableName = edmModel.Container.GetEntitySetByName(a.Constraint.FromRole.Name, true).Table,
                    ToTableName = edmModel.Container.GetEntitySetByName(a.Constraint.ToRole.Name, true).Table
                });

                var tableStatementBuilder = new CreateTableStatementBuilder(entitySet, b);
                yield return tableStatementBuilder.BuildStatement();
            }
        }

        private IEnumerable<CreateIndexStatementCollection> GetCreateIndexStatements()
        {
            foreach (var entitySet in edmModel.Container.EntitySets)
            {
                var indexStatementBuilder = new CreateIndexStatementBuilder(entitySet);
                yield return indexStatementBuilder.BuildStatement();
            }
        }
    }
}