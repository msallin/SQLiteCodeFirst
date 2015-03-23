using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using SQLite.CodeFirst.Statement;

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
            var createDatabaseStatement = new CreateDatabaseStatement
            {
                CreateTableStatements = GetCreateTableStatements()
            };

            return createDatabaseStatement;
        }

        private IEnumerable<CreateTableStatement> GetCreateTableStatements()
        {
            foreach (var entityType in edmModel.EntityTypes)
            {
                ICollection<AssociationType> associationTypes =
                    edmModel.AssociationTypes.Where(a => a.Constraint.ToRole.Name == entityType.Name).ToList();

                var tableStatementBuilder = new CreateTableStatementBuilder(entityType, associationTypes);
                yield return tableStatementBuilder.BuildStatement();
            }
        }
    }
}
