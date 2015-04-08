using System.Data.Entity;
using System.IO;

namespace SQLite.CodeFirst
{
    public class SqliteCreateDatabaseIfNotExists<TContext> : SqliteInitializerBase<TContext>
        where TContext : DbContext
    {
        public SqliteCreateDatabaseIfNotExists(string connectionString, DbModelBuilder modelBuilder)
            : base(connectionString, modelBuilder) { }

        public override void InitializeDatabase(TContext context)
        {
            bool dbExists = File.Exists(DatabaseFilePath);
            if (dbExists)
            {
                return;
            }

            base.InitializeDatabase(context);
        }
    }
}
