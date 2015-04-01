using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SQLite.CodeFirst
{
    public abstract class SqliteInitializerBase<TContext> : IDatabaseInitializer<TContext>
        where TContext : DbContext
    {
        protected readonly DbModelBuilder ModelBuilder;
        protected readonly string DatabaseFilePath;

        protected SqliteInitializerBase(string connectionString, DbModelBuilder modelBuilder)
        {
            DatabaseFilePath = SqliteConnectionStringParser.GetDataSource(connectionString);
            ModelBuilder = modelBuilder;

            // This convention will crash the SQLite Provider before "InitializeDatabase" gets called.
            // See https://github.com/msallin/SQLiteCodeFirst/issues/7 for details.
            modelBuilder.Conventions.Remove<TimestampAttributeConvention>();
        }

        public virtual void InitializeDatabase(TContext context)
        {
            var model = ModelBuilder.Build(context.Database.Connection);

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
                    context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        protected virtual void Seed(TContext context) { }
    }
}