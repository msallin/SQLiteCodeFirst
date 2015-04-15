using System.Data.Entity;
using System.IO;

namespace SQLite.CodeFirst
{
    public class SqliteCreateDatabaseIfNotExists<TContext> : SqliteInitializerBase<TContext>
        where TContext : DbContext
    {
        public SqliteCreateDatabaseIfNotExists(DbModelBuilder modelBuilder)
            : base(modelBuilder) { }

        public override void InitializeDatabase(TContext context)
        {
            string databseFilePath = GetDatabasePathFromContext(context);

            bool dbExists = File.Exists(databseFilePath);
            if (dbExists)
            {
                return;
            }

            base.InitializeDatabase(context);
        }
    }
}
