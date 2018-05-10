using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Statement.ColumnConstraint;

namespace SQLite.CodeFirst.Test.UnitTests.Statement.ColumnConstraint
{
    [TestClass]
    public class NotNullConstraintTest : StatementTestBase
    {
        [TestMethod]
        public void CreateStatementTest()
        {
            var notNullConstraint = new NotNullConstraint();
            string output = notNullConstraint.CreateStatement();
            Assert.AreEqual(output, "NOT NULL");
        }
    }
}
