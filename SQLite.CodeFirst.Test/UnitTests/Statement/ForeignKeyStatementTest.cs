using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst.Test.UnitTests.Statement
{
    [TestClass]
    public class ForeignKeyStatementTest : StatementTestBase
    {
        [TestMethod]
        public void CreateStatementOneForeignKeyTest()
        {
            var foreignKeyStatement = new ForeignKeyStatement
            {
                CascadeDelete = false,
                ForeignKey = new List<string> { "dummyForeignKey1" },
                ForeignPrimaryKey = new List<string> { "dummForeignPrimaryKey1" },
                ForeignTable = "dummyForeignTable"
            };

            string output = foreignKeyStatement.CreateStatement();
            Assert.AreEqual("FOREIGN KEY ([dummyForeignKey1]) REFERENCES dummyForeignTable([dummForeignPrimaryKey1])", output);
        }

        [TestMethod]
        public void CreateStatementOneForeignKeyCascadeDeleteTest()
        {
            var foreignKeyStatement = new ForeignKeyStatement
            {
                CascadeDelete = true,
                ForeignKey = new List<string> { "dummyForeignKey1" },
                ForeignPrimaryKey = new List<string> { "dummForeignPrimaryKey1" },
                ForeignTable = "dummyForeignTable"
            };

            string output = foreignKeyStatement.CreateStatement();
            Assert.AreEqual("FOREIGN KEY ([dummyForeignKey1]) REFERENCES dummyForeignTable([dummForeignPrimaryKey1]) ON DELETE CASCADE", output);
        }

        [TestMethod]
        public void CreateStatementOneForeignKeyCascadeUpdateTest()
        {
            var foreignKeyStatement = new ForeignKeyStatement
            {
                CascadeUpdate = true,
                ForeignKey = new List<string> { "dummyForeignKey1" },
                ForeignPrimaryKey = new List<string> { "dummForeignPrimaryKey1" },
                ForeignTable = "dummyForeignTable"
            };

            string output = foreignKeyStatement.CreateStatement();
            Assert.AreEqual("FOREIGN KEY ([dummyForeignKey1]) REFERENCES dummyForeignTable([dummForeignPrimaryKey1]) ON UPDATE CASCADE", output);
        }

        [TestMethod]
        public void CreateStatementOneForeignKeyCascadeDeleteAndUpdateTest()
        {
            var foreignKeyStatement = new ForeignKeyStatement
            {
                CascadeDelete = true,
                CascadeUpdate = true,
                ForeignKey = new List<string> { "dummyForeignKey1" },
                ForeignPrimaryKey = new List<string> { "dummForeignPrimaryKey1" },
                ForeignTable = "dummyForeignTable"
            };

            string output = foreignKeyStatement.CreateStatement();
            Assert.AreEqual("FOREIGN KEY ([dummyForeignKey1]) REFERENCES dummyForeignTable([dummForeignPrimaryKey1]) ON DELETE CASCADE ON UPDATE CASCADE", output);
        }

        [TestMethod]
        public void CreateStatementTwoForeignKeyTest()
        {
            var foreignKeyStatement = new ForeignKeyStatement
            {
                CascadeDelete = false,
                ForeignKey = new List<string> { "dummyForeignKey1", "dummyForeignKey2" },
                ForeignPrimaryKey = new List<string> { "dummForeignPrimaryKey1" },
                ForeignTable = "dummyForeignTable"
            };

            string output = foreignKeyStatement.CreateStatement();
            Assert.AreEqual("FOREIGN KEY ([dummyForeignKey1], [dummyForeignKey2]) REFERENCES dummyForeignTable([dummForeignPrimaryKey1])", output);
        }

        [TestMethod]
        public void CreateStatementTwoForeignKeyTwoPrimaryKeyTest()
        {
            var foreignKeyStatement = new ForeignKeyStatement
            {
                CascadeDelete = false,
                ForeignKey = new List<string> { "dummyForeignKey1", "dummyForeignKey2" },
                ForeignPrimaryKey = new List<string> { "dummForeignPrimaryKey1", "dummForeignPrimaryKey2" },
                ForeignTable = "dummyForeignTable"
            };

            string output = foreignKeyStatement.CreateStatement();
            Assert.AreEqual("FOREIGN KEY ([dummyForeignKey1], [dummyForeignKey2]) REFERENCES dummyForeignTable([dummForeignPrimaryKey1], [dummForeignPrimaryKey2])", output);
        }

        [TestMethod]
        public void CreateStatementOneForeignKeyTwoPrimaryKeyTest()
        {
            var foreignKeyStatement = new ForeignKeyStatement
            {
                CascadeDelete = false,
                ForeignKey = new List<string> { "dummyForeignKey1" },
                ForeignPrimaryKey = new List<string> { "dummForeignPrimaryKey1", "dummForeignPrimaryKey2" },
                ForeignTable = "dummyForeignTable"
            };

            string output = foreignKeyStatement.CreateStatement();
            Assert.AreEqual("FOREIGN KEY ([dummyForeignKey1]) REFERENCES dummyForeignTable([dummForeignPrimaryKey1], [dummForeignPrimaryKey2])", output);
        }
    }
}
