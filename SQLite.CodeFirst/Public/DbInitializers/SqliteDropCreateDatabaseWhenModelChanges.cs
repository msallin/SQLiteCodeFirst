using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using SQLite.CodeFirst.Utility;
using System.Diagnostics.CodeAnalysis;

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
        /// Generates the SQLite-DDL from the model and executes it against the database.
        /// After that the <see cref="Seed" /> method is executed.
        /// All actions are be executed in transactions.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void InitializeDatabase(TContext context)
        {
            string databaseFilePath = GetDatabasePathFromContext(context);

            bool dbExists = InMemoryAwareFile.Exists(databaseFilePath);
            if (dbExists)
            {
                if (IsSameModel(context))
                {
                    return;
                }

                FileAttributes? attributes = InMemoryAwareFile.GetFileAttributes(databaseFilePath);
                CloseDatabase(context);
                DeleteDatabase(context, databaseFilePath);
                base.InitializeDatabase(context);
                InMemoryAwareFile.SetFileAttributes(databaseFilePath, attributes);
                SaveHistory(context);
            }
            else
            {
                base.InitializeDatabase(context);
                SaveHistory(context);
            }
        }

        /// <summary>
        /// Called to drop/remove Database file from disk.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="databaseFilePath">Filename of Database to be removed.</param>
        protected virtual void DeleteDatabase(TContext context, string databaseFilePath)
        {
            InMemoryAwareFile.Delete(databaseFilePath);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.GC.Collect", Justification = "Required.")]
        private static void CloseDatabase(TContext context)
        {
            context.Database.Connection.Close();
            GC.Collect();
            GC.WaitForPendingFinalizers();
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

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private bool IsSameModel(TContext context)
        {

            var hash = GetHashFromModel(context.Database.Connection);

            try
            {
                var history = GetHistoryRecord(context);
                return history?.Hash == hash;
            }
            catch (Exception)
            {
                // This happens if the history table does not exist.
                // So it covers also the case with a null byte file (see SqliteCreateDatabaseIfNotExists).
                return false;
            }
        }

        private IHistory GetHistoryRecord(TContext context)
        {
            // Yes, it seams to be complicated but it has to be done this way
            // in order to be supported by .NET 4.0.
            DbQuery dbQuery = context.Set(historyEntityType).AsNoTracking();
            IEnumerable<IHistory> records = Enumerable.Cast<IHistory>(dbQuery);
            return records.SingleOrDefault();
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
            var sqliteSqlGenerator = new SqliteSqlGenerator();
            return sqliteSqlGenerator.Generate(model.StoreModel);
        }
    }
}
