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
    public class SqlGenerationDefaultCollationTest
    {
        private const string ReferenceSql =
            @"
CREATE TABLE ""MyTable"" ([Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, [Name] nvarchar NOT NULL COLLATE custom_collate, FOREIGN KEY ([Id]) REFERENCES ""Coaches""([Id]));
CREATE TABLE ""Coaches"" ([Id] INTEGER PRIMARY KEY, [FirstName] nvarchar (50) COLLATE NOCASE, [LastName] nvarchar (50) COLLATE custom_collate, [Street] nvarchar (100) COLLATE custom_collate, [City] nvarchar NOT NULL COLLATE custom_collate, [CreatedUtc] datetime NOT NULL DEFAULT (DATETIME('now')));
CREATE TABLE ""TeamPlayer"" ([Id] INTEGER PRIMARY KEY, [Number] int NOT NULL, [TeamId] int NOT NULL, [FirstName] nvarchar (50) COLLATE NOCASE, [LastName] nvarchar (50) COLLATE custom_collate, [Street] nvarchar (100) COLLATE custom_collate, [City] nvarchar NOT NULL COLLATE custom_collate, [CreatedUtc] datetime NOT NULL DEFAULT (DATETIME('now')), [Mentor_Id] int, FOREIGN KEY ([Mentor_Id]) REFERENCES ""TeamPlayer""([Id]), FOREIGN KEY ([TeamId]) REFERENCES ""MyTable""([Id]) ON DELETE CASCADE);
CREATE TABLE ""Stadions"" ([Name] nvarchar (128) NOT NULL COLLATE custom_collate, [Street] nvarchar (128) NOT NULL COLLATE custom_collate, [City] nvarchar (128) NOT NULL COLLATE custom_collate, [Order] int NOT NULL, [Team_Id] int NOT NULL, PRIMARY KEY([Name], [Street], [City]), FOREIGN KEY ([Team_Id]) REFERENCES ""MyTable""([Id]) ON DELETE CASCADE);
CREATE TABLE ""Foos"" ([FooId] INTEGER PRIMARY KEY, [Name] nvarchar COLLATE custom_collate, [FooSelf1Id] int, [FooSelf2Id] int, [FooSelf3Id] int, FOREIGN KEY ([FooSelf1Id]) REFERENCES ""Foos""([FooId]), FOREIGN KEY ([FooSelf2Id]) REFERENCES ""Foos""([FooId]), FOREIGN KEY ([FooSelf3Id]) REFERENCES ""Foos""([FooId]));
CREATE TABLE ""FooSelves"" ([FooSelfId] INTEGER PRIMARY KEY, [FooId] int NOT NULL, [Number] int NOT NULL, FOREIGN KEY ([FooId]) REFERENCES ""Foos""([FooId]) ON DELETE CASCADE);
CREATE TABLE ""FooSteps"" ([FooStepId] INTEGER PRIMARY KEY, [FooId] int NOT NULL, [Number] int NOT NULL, FOREIGN KEY ([FooId]) REFERENCES ""Foos""([FooId]) ON DELETE CASCADE);
CREATE TABLE ""FooCompositeKeys"" ([Id] int NOT NULL, [Version] nvarchar (20) NOT NULL COLLATE custom_collate, [Name] nvarchar (255) COLLATE custom_collate, PRIMARY KEY([Id], [Version]));
CREATE TABLE ""FooRelationshipAs"" ([Id] INTEGER PRIMARY KEY, [Name] nvarchar (255) COLLATE custom_collate);
CREATE TABLE ""FooRelationshipBs"" ([Id] INTEGER PRIMARY KEY, [Name] nvarchar (255) COLLATE custom_collate);
CREATE TABLE ""FooRelationshipAFooCompositeKeys"" ([FooRelationshipA_Id] int NOT NULL, [FooCompositeKey_Id] int NOT NULL, [FooCompositeKey_Version] nvarchar (20) NOT NULL COLLATE custom_collate, PRIMARY KEY([FooRelationshipA_Id], [FooCompositeKey_Id], [FooCompositeKey_Version]), FOREIGN KEY ([FooRelationshipA_Id]) REFERENCES ""FooRelationshipAs""([Id]) ON DELETE CASCADE, FOREIGN KEY ([FooCompositeKey_Id], [FooCompositeKey_Version]) REFERENCES ""FooCompositeKeys""([Id], [Version]) ON DELETE CASCADE);
CREATE TABLE ""FooRelationshipBFooCompositeKeys"" ([FooRelationshipB_Id] int NOT NULL, [FooCompositeKey_Id] int NOT NULL, [FooCompositeKey_Version] nvarchar (20) NOT NULL COLLATE custom_collate, PRIMARY KEY([FooRelationshipB_Id], [FooCompositeKey_Id], [FooCompositeKey_Version]), FOREIGN KEY ([FooRelationshipB_Id]) REFERENCES ""FooRelationshipBs""([Id]) ON DELETE CASCADE, FOREIGN KEY ([FooCompositeKey_Id], [FooCompositeKey_Version]) REFERENCES ""FooCompositeKeys""([Id], [Version]) ON DELETE CASCADE);
CREATE  INDEX ""IX_MyTable_Id"" ON ""MyTable"" (""Id"");
CREATE  INDEX ""IX_Team_TeamsName"" ON ""MyTable"" (""Name"");
CREATE  INDEX ""IX_TeamPlayer_Number"" ON ""TeamPlayer"" (""Number"");
CREATE UNIQUE INDEX ""IX_TeamPlayer_NumberPerTeam"" ON ""TeamPlayer"" (""Number"", ""TeamId"");
CREATE  INDEX ""IX_TeamPlayer_Mentor_Id"" ON ""TeamPlayer"" (""Mentor_Id"");
CREATE UNIQUE INDEX ""IX_Stadion_Main"" ON ""Stadions"" (""Street"", ""Name"");
CREATE UNIQUE INDEX ""ReservedKeyWordTest"" ON ""Stadions"" (""Order"");
CREATE  INDEX ""IX_Stadion_Team_Id"" ON ""Stadions"" (""Team_Id"");
CREATE  INDEX ""IX_Foo_FooSelf1Id"" ON ""Foos"" (""FooSelf1Id"");
CREATE  INDEX ""IX_Foo_FooSelf2Id"" ON ""Foos"" (""FooSelf2Id"");
CREATE  INDEX ""IX_Foo_FooSelf3Id"" ON ""Foos"" (""FooSelf3Id"");
CREATE  INDEX ""IX_FooSelf_FooId"" ON ""FooSelves"" (""FooId"");
CREATE  INDEX ""IX_FooStep_FooId"" ON ""FooSteps"" (""FooId"");
CREATE  INDEX ""IX_FooRelationshipAFooCompositeKey_FooRelationshipA_Id"" ON ""FooRelationshipAFooCompositeKeys"" (""FooRelationshipA_Id"");
CREATE  INDEX ""IX_FooRelationshipAFooCompositeKey_FooCompositeKey_Id_FooCompositeKey_Version"" ON ""FooRelationshipAFooCompositeKeys"" (""FooCompositeKey_Id"", ""FooCompositeKey_Version"");
CREATE  INDEX ""IX_FooRelationshipBFooCompositeKey_FooRelationshipB_Id"" ON ""FooRelationshipBFooCompositeKeys"" (""FooRelationshipB_Id"");
CREATE  INDEX ""IX_FooRelationshipBFooCompositeKey_FooCompositeKey_Id_FooCompositeKey_Version"" ON ""FooRelationshipBFooCompositeKeys"" (""FooCompositeKey_Id"", ""FooCompositeKey_Version"");
";

        private static string generatedSql;

        // Does not work on the build server. No clue why.

        [TestMethod]
        public void SqliteSqlGeneratorWithDefaultCollationTest()
        {
            using (DbConnection connection = new SQLiteConnection("FullUri=file::memory:"))
            {
                // This is important! Else the in memory database will not work.
                connection.Open();

                var defaultCollation = new Collation() { Function = CollationFunction.Custom, CustomFunction  = "custom_collate" };
                using (var context = new DummyDbContext(connection, defaultCollation))
                {
                    // ReSharper disable once UnusedVariable
                    Player fo = context.Set<Player>().FirstOrDefault();

                    Assert.AreEqual(RemoveLineEndings(ReferenceSql), RemoveLineEndings(generatedSql));
                }
            }
        }

        private static string RemoveLineEndings(string input)
        {
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();
            return input.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(lineSeparator, string.Empty).Replace(paragraphSeparator, string.Empty);
        }

        private class DummyDbContext : DbContext
        {
            private readonly Collation defaultCollation;

            public DummyDbContext(DbConnection connection, Collation defaultCollation = null)
                : base(connection, false)
            {
                this.defaultCollation = defaultCollation;
            }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                // This configuration contains all supported cases.
                // So it makes a perfect test to validate whether the 
                // generated SQL is correct.
                ModelConfiguration.Configure(modelBuilder);
                var initializer = new AssertInitializer(modelBuilder, defaultCollation);
                Database.SetInitializer(initializer);
            }

            private class AssertInitializer : SqliteInitializerBase<DummyDbContext>
            {
                private readonly Collation defaultCollation;

                public AssertInitializer(DbModelBuilder modelBuilder, Collation defaultCollation)
                    : base(modelBuilder)
                {
                    this.defaultCollation = defaultCollation;
                }

                public override void InitializeDatabase(DummyDbContext context)
                {
                    DbModel model = ModelBuilder.Build(context.Database.Connection);
                    var sqliteSqlGenerator = new SqliteSqlGenerator(defaultCollation);
                    generatedSql = sqliteSqlGenerator.Generate(model.StoreModel);
                    base.InitializeDatabase(context);
                }
            }
        }
    }
}