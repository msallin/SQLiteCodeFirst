using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using SQLite.CodeFirst.Builder.NameCreators;
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

                IList<AssociationTypeWrapper> associationTypeWrappers = new List<AssociationTypeWrapper>();
                foreach (var associationType in associationTypes)
                {
                    string fromTable = edmModel.Container.GetEntitySetByName(associationType.Constraint.FromRole.Name, true).Table;
                    string toTable = edmModel.Container.GetEntitySetByName(associationType.Constraint.ToRole.Name, true).Table;

                    string fromTableName = TableNameCreator.CreateTableName(fromTable);
                    string toTableName = TableNameCreator.CreateTableName(toTable);

                    associationTypeWrappers.Add(new AssociationTypeWrapper
                    {
                        AssociationType = associationType,
                        FromTableName = fromTableName,
                        ToTableName = toTableName
                    });
                }

                var tableStatementBuilder = new CreateTableStatementBuilder(entitySet, associationTypeWrappers);
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