using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.NetCore.Console;

namespace SQLite.CodeFirst.Test.UnitTests.DbInitializers
{
    /// <summary>
    /// Verifies that the public initializers forward an explicitly supplied default
    /// <see cref="Collation"/> to the base class, which is what feeds it into the SQL generation.
    /// Without this the documented "default collation" feature is unreachable through the shipped initializers.
    /// </summary>
    [TestClass]
    public class InitializerDefaultCollationTest
    {
        private static readonly Collation Collation = new Collation(CollationFunction.RTrim);

        private static DbModelBuilder NewModelBuilder()
        {
            return new DbModelBuilder();
        }

        [TestMethod]
        public void SqliteCreateDatabaseIfNotExists_ForwardsDefaultCollation()
        {
            var initializer = new SqliteCreateDatabaseIfNotExists<FootballDbContext>(NewModelBuilder(), Collation);
            Assert.AreSame(Collation, initializer.DefaultCollation);
        }

        [TestMethod]
        public void SqliteCreateDatabaseIfNotExists_WithNullByteFlag_ForwardsDefaultCollation()
        {
            var initializer = new SqliteCreateDatabaseIfNotExists<FootballDbContext>(NewModelBuilder(), true, Collation);
            Assert.AreSame(Collation, initializer.DefaultCollation);
        }

        [TestMethod]
        public void SqliteDropCreateDatabaseAlways_ForwardsDefaultCollation()
        {
            var initializer = new SqliteDropCreateDatabaseAlways<FootballDbContext>(NewModelBuilder(), Collation);
            Assert.AreSame(Collation, initializer.DefaultCollation);
        }

        [TestMethod]
        public void SqliteDropCreateDatabaseWhenModelChanges_ForwardsDefaultCollation()
        {
            var initializer = new SqliteDropCreateDatabaseWhenModelChanges<FootballDbContext>(NewModelBuilder(), Collation);
            Assert.AreSame(Collation, initializer.DefaultCollation);
        }

        [TestMethod]
        public void SqliteDropCreateDatabaseWhenModelChanges_WithHistoryType_ForwardsDefaultCollation()
        {
            var initializer = new SqliteDropCreateDatabaseWhenModelChanges<FootballDbContext>(NewModelBuilder(), typeof(History), Collation);
            Assert.AreSame(Collation, initializer.DefaultCollation);
        }

        [TestMethod]
        public void Initializer_WithoutCollation_HasNullDefaultCollation()
        {
            var initializer = new SqliteDropCreateDatabaseAlways<FootballDbContext>(NewModelBuilder());
            Assert.IsNull(initializer.DefaultCollation);
        }
    }
}
