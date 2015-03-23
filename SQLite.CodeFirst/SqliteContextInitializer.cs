using System;
using System.Data.Entity;
using System.IO;

namespace SQLite.CodeFirst
{
    public class SqliteContextInitializer<TDbContext> : IDatabaseInitializer<TDbContext>
        where TDbContext : DbContext
    {
        protected readonly bool dbExists;
        protected readonly DbModelBuilder modelBuilder;

        public SqliteContextInitializer(string connectionString, DbModelBuilder modelBuilder)
        {
            string path = SqliteConnectionStringParser.GetDataSource(connectionString);
            dbExists = File.Exists(path);
            this.modelBuilder = modelBuilder;
        }

        public virtual void InitializeDatabase(TDbContext context)
        {
            if (dbExists)
            {
                return;
            }

            var model = modelBuilder.Build(context.Database.Connection);

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var sqliteDatabaseCreator = new SqliteDatabaseCreator(context.Database, model);
                    sqliteDatabaseCreator.Create();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Seed(context);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        protected virtual void Seed(TDbContext context) { }
    }
}