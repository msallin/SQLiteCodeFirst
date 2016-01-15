using System.Data.Entity.Core.Metadata.Edm;
using SQLite.CodeFirst.Builder;
using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst
{
    /// <summary>
    /// Generates the SQL statement to create a database, based on a <see cref="EdmModel"/>.
    /// </summary>
    public class SqliteSqlGenerator : ISqlGenerator
    {
        private readonly EdmModel storeModel;

        public SqliteSqlGenerator(EdmModel storeModel)
        {
            this.storeModel = storeModel;
        }

        /// <summary>
        /// Generates the SQL statement.
        /// </summary>
        public string Generate()
        {
            IStatementBuilder<CreateDatabaseStatement> statementBuilder = new CreateDatabaseStatementBuilder(storeModel);
            IStatement statement = statementBuilder.BuildStatement();
            return statement.CreateStatement();
        }
    }
}
