using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst.Test.UnitTests.Statement
{
    [TestClass]
    public class PrimaryKeyStatementTest : StatementTestBase
    {
        [TestMethod]
        public void CreateStatementWithOneKeyTest()
        {
            const string keyMember1 = "keyMember1";

            var primaryKeyStatement = new CompositePrimaryKeyStatement(new List<string> { keyMember1 });
            Assert.AreEqual(1, primaryKeyStatement.Count);
            Assert.AreEqual("PRIMARY KEY([keyMember1])", primaryKeyStatement.CreateStatement());
        }

        [TestMethod]
        public void CreateStatementWithTwoKeyTest()
        {
            const string keyMember1 = "keyMember1";
            const string keyMember2 = "keyMember2";

            var primaryKeyStatement = new CompositePrimaryKeyStatement(new List<string> { keyMember1, keyMember2 });
            Assert.AreEqual(2, primaryKeyStatement.Count);
            Assert.AreEqual("PRIMARY KEY([keyMember1], [keyMember2])", primaryKeyStatement.CreateStatement());
        }
    }
}
