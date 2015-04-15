using System.Data.Entity;
using System.IO;

namespace SQLite.CodeFirst
{
    public class SqliteDropCreateDatabaseAlways<TContext> : SqliteInitializerBase<TContext>
        where TContext : DbContext
    {
        public SqliteDropCreateDatabaseAlways(DbModelBuilder modelBuilder)
            : base(modelBuilder) { }

        public override void InitializeDatabase(TContext context)
        {
            string databseFilePath = GetDatabasePathFromContext(context);

            bool dbExists = File.Exists(databseFilePath);
            if (dbExists)
            {
                File.Delete(databseFilePath);
            }

            base.InitializeDatabase(context);
        }
    }
}
