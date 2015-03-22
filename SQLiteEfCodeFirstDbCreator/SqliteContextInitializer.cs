using System;
using System.Data.Entity;
using System.IO;

namespace SQLiteEfCodeFirstDbCreator
{
    public class SqliteContextInitializer<T> : IDatabaseInitializer<T>
        where T : DbContext
    {
        private readonly bool dbExists;
        private readonly DbModelBuilder modelBuilder;

        public SqliteContextInitializer(string dbPath, DbModelBuilder modelBuilder)
        {
            dbExists = File.Exists(dbPath);
            this.modelBuilder = modelBuilder;
        }

        public void InitializeDatabase(T context)
        {
            if (dbExists)
                return;

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
        }
    }
}
