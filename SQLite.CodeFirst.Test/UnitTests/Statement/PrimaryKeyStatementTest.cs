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
            Assert.AreEqual(primaryKeyStatement.Count, 1);
            Assert.AreEqual(primaryKeyStatement.CreateStatement(), "PRIMARY KEY([keyMember1])");
        }

        [TestMethod]
        public void CreateStatementWithTwoKeyTest()
        {
            const string keyMember1 = "keyMember1";
            const string keyMember2 = "keyMember2";

            var primaryKeyStatement = new CompositePrimaryKeyStatement(new List<string> { keyMember1, keyMember2 });
            Assert.AreEqual(primaryKeyStatement.Count, 2);
            Assert.AreEqual(primaryKeyStatement.CreateStatement(), "PRIMARY KEY([keyMember1], [keyMember2])");
        }
    }
}
