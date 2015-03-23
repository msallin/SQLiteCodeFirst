using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using SQLite.CodeFirst.Builder;
using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst
{
    public class SqliteDatabaseCreator
    {
        private readonly Database db;
        private readonly DbModel model;

        public SqliteDatabaseCreator(Database db, DbModel model)
        {
            this.db = db;
            this.model = model;
        }

        public void Create()
        {
            IStatementBuilder<CreateDatabaseStatement> statementBuilder = new CreateDatabaseStatementBuilder(model.StoreModel);
            IStatement statement = statementBuilder.BuildStatement();
            string sql = statement.CreateStatement();
            db.ExecuteSqlCommand(sql);
        }
    }
}
