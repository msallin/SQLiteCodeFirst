using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using SQLite.CodeFirst.Builder;
using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst
{
    /// <summary>
    /// Creates a SQLite-Database based on a Entity Framework <see cref="Database"/> and <see cref="DbModel"/>.
    /// This creator can be used standalone or within an initializer.
    /// </summary>
    public class SqliteDatabaseCreator
    {
        private readonly Database db;
        private readonly DbModel model;

        public SqliteDatabaseCreator(Database db, DbModel model)
        {
            this.db = db;
            this.model = model;
        }

        /// <summary>
        /// Creates the SQLite-Database.
        /// </summary>
        public void Create()
        {
            IStatementBuilder<CreateDatabaseStatement> statementBuilder = new CreateDatabaseStatementBuilder(model.StoreModel);
            IStatement statement = statementBuilder.BuildStatement();
            string sql = statement.CreateStatement();
            db.ExecuteSqlCommand(sql);
        }
    }
}
