using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst.Test.UnitTests.Statement
{
    [TestClass]
    public class CreateDatabaseStatementTest : StatementTestBase
    {
        [TestMethod]
        public void CreateStatementWithOneKeyTest()
        {
            var statements = new List<IStatement>
            {
                CreateStatementMock("dummy1").Object
            };

            var createDatabaseStatement = new CreateDatabaseStatement(statements);
            Assert.AreEqual(createDatabaseStatement.Count, 1);
            Assert.AreEqual(createDatabaseStatement.CreateStatement(), "dummy1");
        }

        [TestMethod]
        public void CreateStatementWithTwoKeyTest()
        {
            var statements = new List<IStatement>
            {
                CreateStatementMock("dummy1").Object,
                CreateStatementMock("dummy2").Object
            };

            var createDatabaseStatement = new CreateDatabaseStatement(statements);
            Assert.AreEqual(createDatabaseStatement.Count, 2);
            Assert.AreEqual(createDatabaseStatement.CreateStatement(), "dummy1\r\ndummy2");
        }
    }
}
