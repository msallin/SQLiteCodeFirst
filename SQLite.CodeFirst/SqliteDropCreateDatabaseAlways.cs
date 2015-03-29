using System.Data.Entity;
using System.IO;

namespace SQLite.CodeFirst
{
    public class SqliteDropCreateDatabaseAlways<TContext> : SqliteInitializerBase<TContext>
        where TContext : DbContext
    {
        public SqliteDropCreateDatabaseAlways(string connectionString, DbModelBuilder modelBuilder)
            : base(connectionString, modelBuilder) { }

        public override void InitializeDatabase(TContext context)
        {
            bool dbExists = File.Exists(DatabaseFilePath);
            if (dbExists)
            {
                File.Delete(DatabaseFilePath);
            }

            base.InitializeDatabase(context);
        }
    }
}
