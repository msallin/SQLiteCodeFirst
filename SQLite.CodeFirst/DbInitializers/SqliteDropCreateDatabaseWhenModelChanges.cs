using System;
using System.Data.Common;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SQLite.CodeFirst.Utility;

namespace SQLite.CodeFirst
{
    /// <summary>
    /// An implementation of <see cref="IDatabaseInitializer{TContext}"/> that will always recreate and optionally re-seed the 
    /// database the first time that a context is used in the app domain or if the model has changed. 
    /// To seed the database, create a derived class and override the Seed method.
    /// <remarks>
    /// To detect model changes a new table (implementation of <see cref="IHistory"/>) is added to the database.
    /// There is one record in this table which holds the hash of the SQL-statement which was generated from the model
    /// executed to create the database. When initializing the database the initializer checks if the hash of the SQL-statement for the
    /// model is still the same as the hash in the database.  If you use this initializer on a existing database, this initializer 
    /// will interpret this as model change because of the new <see cref="IHistory"/> table.
    /// Notice that a database can be used by more than one context. Therefore the name of the context is saved as a part of the history record.
    /// </remarks>
    /// </summary>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public class SqliteDropCreateDatabaseWhenModelChanges<TContext> : SqliteInitializerBase<TContext>
        where TContext : DbContext
    {
        private readonly Type historyEntityType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteDropCreateDatabaseWhenModelChanges{TContext}"/> class.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        public SqliteDropCreateDatabaseWhenModelChanges(DbModelBuilder modelBuilder)
            : base(modelBuilder)
        {
            historyEntityType = typeof(History);
            ConfigureHistoryEntity();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteDropCreateDatabaseWhenModelChanges{TContext}"/> class.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="historyEntityType">Type of the history entity (must implement <see cref="IHistory"/> and provide an parameterless constructor).</param>
        public SqliteDropCreateDatabaseWhenModelChanges(DbModelBuilder modelBuilder, Type historyEntityType)
            : base(modelBuilder)
        {
            this.historyEntityType = historyEntityType;
            ConfigureHistoryEntity();
        }


        protected void ConfigureHistoryEntity()
        {
            HistoryEntityTypeValidator.EnsureValidType(historyEntityType);
            ModelBuilder.RegisterEntityType(historyEntityType);
        }

        /// <summary>
        /// Initialize the database for the given context.
        /// Generates the SQLite-DDL from the model and executs it against the database.
        /// After that the <see cref="Seed" /> method is executed.
        /// All actions are be executed in transactions.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void InitializeDatabase(TContext context)
        {
            string databseFilePath = GetDatabasePathFromContext(context);

            bool dbExists = File.Exists(databseFilePath);
            if (dbExists)
            {
                if (IsSameModel(context))
                {
                    return;
                }

                DeleteDatabase(context, databseFilePath);
                base.InitializeDatabase(context);
                SaveHistory(context);
            }
            else
            {
                base.InitializeDatabase(context);
                SaveHistory(context);
            }
        }

        private static void DeleteDatabase(TContext context, string databseFilePath)
        {
            context.Database.Connection.Close();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            File.Delete(databseFilePath);
        }

        private void SaveHistory(TContext context)
        {
            var hash = GetHashFromModel(context.Database.Connection);
            var history = GetHistoryRecord(context);
            EntityState entityState;
            if (history == null)
            {
                history = (IHistory)Activator.CreateInstance(historyEntityType);
                entityState = EntityState.Added;
            }
            else
            {
                entityState = EntityState.Modified;
            }

            history.Context = context.GetType().FullName;
            history.Hash = hash;
            history.CreateDate = DateTime.UtcNow;

            context.Set(historyEntityType).Attach(history);
            context.Entry(history).State = entityState;
            context.SaveChanges();
        }

        private bool IsSameModel(TContext context)
        {
            try
            {
                var hash = GetHashFromModel(context.Database.Connection);
                var history = GetHistoryRecord(context);
                return history?.Hash == hash;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private IHistory GetHistoryRecord(TContext context)
        {
            return context.Set(historyEntityType)
                .AsNoTracking()
                .ToListAsync()
                .Result
                .Cast<IHistory>()
                .SingleOrDefault();
        }

        private string GetHashFromModel(DbConnection connection)
        {
            var sql = GetSqlFromModel(connection);
            string hash = HashCreator.CreateHash(sql);
            return hash;
        }

        private string GetSqlFromModel(DbConnection connection)
        {
            var model = ModelBuilder.Build(connection);
            var sqliteSqlGenerator = new SqliteSqlGenerator(model.StoreModel);
            return sqliteSqlGenerator.Generate();
        }
    }
}
