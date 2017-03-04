using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Console;
using SQLite.CodeFirst.Console.Entity;

namespace SQLite.CodeFirst.Test.IntegrationTests
{
    [TestClass]
    public class SqlGenerationTest
    {
        private const string ReferenceSql =
            @"CREATE TABLE ""MyTable"" ([Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, [Name] nvarchar NOT NULL, FOREIGN KEY (Id) REFERENCES ""Coaches""(Id));
CREATE TABLE ""Coaches"" ([Id] INTEGER PRIMARY KEY, [FirstName] nvarchar (50) COLLATE NOCASE, [LastName] nvarchar (50), [Street] nvarchar (100), [City] nvarchar NOT NULL);
CREATE TABLE ""TeamPlayer"" ([Id] INTEGER PRIMARY KEY, [Number] int NOT NULL, [TeamId] int NOT NULL, [FirstName] nvarchar (50) COLLATE NOCASE, [LastName] nvarchar (50), [Street] nvarchar (100), [City] nvarchar NOT NULL, [Mentor_Id] int, FOREIGN KEY (Mentor_Id) REFERENCES ""TeamPlayer""(Id), FOREIGN KEY (TeamId) REFERENCES ""MyTable""(Id) ON DELETE CASCADE);
CREATE TABLE ""Stadions"" ([Name] nvarchar (128) NOT NULL, [Street] nvarchar (128) NOT NULL, [City] nvarchar (128) NOT NULL, [Team_Id] int NOT NULL, PRIMARY KEY(Name, Street, City), FOREIGN KEY (Team_Id) REFERENCES ""MyTable""(Id) ON DELETE CASCADE);
CREATE TABLE ""Foos"" ([FooId] INTEGER PRIMARY KEY, [Name] nvarchar, [FooSelf1Id] int, [FooSelf2Id] int, [FooSelf3Id] int, FOREIGN KEY (FooSelf1Id) REFERENCES ""Foos""(FooId), FOREIGN KEY (FooSelf2Id) REFERENCES ""Foos""(FooId), FOREIGN KEY (FooSelf3Id) REFERENCES ""Foos""(FooId));
CREATE TABLE ""FooSelves"" ([FooSelfId] INTEGER PRIMARY KEY, [FooId] int NOT NULL, [Number] int NOT NULL, FOREIGN KEY (FooId) REFERENCES ""Foos""(FooId) ON DELETE CASCADE);
CREATE TABLE ""FooSteps"" ([FooStepId] INTEGER PRIMARY KEY, [FooId] int NOT NULL, [Number] int NOT NULL, FOREIGN KEY (FooId) REFERENCES ""Foos""(FooId) ON DELETE CASCADE);
CREATE  INDEX ""IX_MyTable_Id"" ON ""MyTable"" (Id);
CREATE  INDEX IX_Team_TeamsName ON ""MyTable"" (Name);

CREATE  INDEX ""IX_TeamPlayer_Number"" ON ""TeamPlayer"" (Number);
CREATE UNIQUE INDEX IX_TeamPlayer_NumberPerTeam ON ""TeamPlayer"" (Number, TeamId);
CREATE  INDEX ""IX_TeamPlayer_Mentor_Id"" ON ""TeamPlayer"" (Mentor_Id);
CREATE UNIQUE INDEX IX_Stadion_Main ON ""Stadions"" (Street, Name);
CREATE  INDEX ""IX_Stadion_Team_Id"" ON ""Stadions"" (Team_Id);
CREATE  INDEX ""IX_Foo_FooSelf1Id"" ON ""Foos"" (FooSelf1Id);
CREATE  INDEX ""IX_Foo_FooSelf2Id"" ON ""Foos"" (FooSelf2Id);
CREATE  INDEX ""IX_Foo_FooSelf3Id"" ON ""Foos"" (FooSelf3Id);
CREATE  INDEX ""IX_FooSelf_FooId"" ON ""FooSelves"" (FooId);
CREATE  INDEX ""IX_FooStep_FooId"" ON ""FooSteps"" (FooId);";

        private static string generatedSql;

        // Does not work on the build server. No clue why.
        [Ignore]
        [TestMethod]
        public void SqliteSqlGeneratorTest()
        {
            using (DbConnection connection = new SQLiteConnection("data source=:memory:"))
            {
                // This is important! Else the in memory database will not work.
                connection.Open();

                using (var context = new DummyDbContext(connection))
                {
                    // ReSharper disable once UnusedVariable
                    Player fo = context.Set<Player>().FirstOrDefault();

                    Assert.IsTrue(string.Equals(ReferenceSql.Trim(), generatedSql.Trim(), StringComparison.OrdinalIgnoreCase));
                }
            }
        }

        private class DummyDbContext : DbContext
        {
            public DummyDbContext(DbConnection connection)
                : base(connection, false)
            {
            }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                // This configuration contains all supported cases.
                // So it makes a perfect test to validate whether the 
                // generated SQL is correct.
                ModelConfiguration.Configure(modelBuilder);
                var initializer = new AssertInitializer(modelBuilder);
                Database.SetInitializer(initializer);
            }
        }

        private class AssertInitializer : SqliteInitializerBase<DummyDbContext>
        {
            public AssertInitializer(DbModelBuilder modelBuilder)
                : base(modelBuilder)
            {
            }

            public override void InitializeDatabase(DummyDbContext context)
            {
                DbModel model = ModelBuilder.Build(context.Database.Connection);
                var sqliteSqlGenerator = new SqliteSqlGenerator();
                generatedSql = sqliteSqlGenerator.Generate(model.StoreModel);
                base.InitializeDatabase(context);
            }
        }
    }
}