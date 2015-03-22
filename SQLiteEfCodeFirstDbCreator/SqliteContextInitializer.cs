using System;
using System.Data.Entity;
using System.IO;
using System.Linq;

namespace SQLiteEfCodeFirstDbCreator
{
    public class SqliteContextInitializer<T> : IDatabaseInitializer<T>
        where T : DbContext
    {
        private readonly bool dbExists;
        private readonly DbModelBuilder modelBuilder;

        public SqliteContextInitializer(string connectionString, DbModelBuilder modelBuilder)
        {
            string path = GetPathFromConnectionString(connectionString);
            dbExists = File.Exists(path);
            this.modelBuilder = modelBuilder;
        }

        private string GetPathFromConnectionString(string connectionString)
        {
            string[] keyValuePairs = connectionString.Split(';');
            string dataSourceKeyValuePair = keyValuePairs.Single(s => s.ToLower().StartsWith("data source"));
            string dataSourceValue = dataSourceKeyValuePair.Split('=')[1];
            return dataSourceValue;
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
