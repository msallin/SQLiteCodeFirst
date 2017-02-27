using System.Data.Entity;
using System.IO;

namespace SQLite.CodeFirst
{
    /// <summary>
    /// An implementation of <see cref="IDatabaseInitializer{TContext}"/> that will recreate and optionally re-seed the
    /// database only if the database does not exist. To seed the database, create a derived class and override the Seed method. 
    /// </summary>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public class SqliteCreateDatabaseIfNotExists<TContext> : SqliteInitializerBase<TContext>
        where TContext : DbContext
    {
        private readonly bool nullByteFileMeansNotExisting;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteCreateDatabaseIfNotExists{TContext}"/> class.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        public SqliteCreateDatabaseIfNotExists(DbModelBuilder modelBuilder)
            : this(modelBuilder, false)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteCreateDatabaseIfNotExists{TContext}"/> class.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        /// <param name="nullByteFileMeansNotExisting">if set to <c>true</c> a null byte database file is treated like the database does not exist.</param>
        public SqliteCreateDatabaseIfNotExists(DbModelBuilder modelBuilder, bool nullByteFileMeansNotExisting)
            : base(modelBuilder)
        {
            this.nullByteFileMeansNotExisting = nullByteFileMeansNotExisting;
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
            string databaseFilePath = GetDatabasePathFromContext(context);

            bool exists;
            if (IsMemoryDb(databaseFilePath))
            {
                //We consider that memory database has to be created each time
                exists = false;
            }
            else
            {
                var fileInfo = new FileInfo(databaseFilePath);

                if (nullByteFileMeansNotExisting)
                {
                    exists = fileInfo.Exists && fileInfo.Length != 0;
                }
                else
                {
                    exists = fileInfo.Exists;
                }
            }

            if (exists)
            {
                return;
            }

            base.InitializeDatabase(context);
        }
    }
}
