using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SQLite.CodeFirst.Test.IntegrationTests
{
    /// <summary>
    ///  Inherit this class to create a test which uses a SQLite in memory database. 
    ///  This class provides the necessary logic to run multiple tests again the in memory database in a row.
    /// </summary>
    public abstract class InMemoryDbTest<TDbContext> 
        where TDbContext : DbContext
    {
        private bool dbInitialized;

        protected DbConnection Connection { get; private set; }

        protected DbContext GetDbContext()
        {
            TDbContext context = (TDbContext)Activator.CreateInstance(typeof(TDbContext), Connection, false);
            if (!dbInitialized)
            {
                context.Database.Initialize(true);
                dbInitialized = true;
            }
            return context;
        }

        [TestInitialize]
        public void Initialize()
        {
            Connection = new SQLiteConnection("data source=:memory:");

            // This is important! Else the in memory database will not work.
            Connection.Open();

            dbInitialized = false;
        }

        [TestCleanup]
        public void Cleanup()
        {
            Connection.Dispose();
        }
    }
}
