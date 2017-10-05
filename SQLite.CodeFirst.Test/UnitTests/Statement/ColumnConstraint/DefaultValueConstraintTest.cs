using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Statement.ColumnConstraint;

namespace SQLite.CodeFirst.Test.UnitTests.Statement.ColumnConstraint
{
    [TestClass]
    public class DefaultValueConstraintTest
    {
        [TestMethod]
        public void CreateStatement_StatementIsCorrect_IntDefault()
        {
            var defaultValueConstraint = new DefaultValueConstraint();
            defaultValueConstraint.DefaultValue = "0";
            string output = defaultValueConstraint.CreateStatement();
            Assert.AreEqual(output, "DEFAULT (0)");
        }

        [TestMethod]
        public void CreateStatement_StatementIsCorrect_StringDefault()
        {
            var defaultValueConstraint = new DefaultValueConstraint();
            defaultValueConstraint.DefaultValue = @"'Something'";
            string output = defaultValueConstraint.CreateStatement();
            Assert.AreEqual(output, "DEFAULT ('Something')");
        }

        [TestMethod]
        public void CreateStatement_StatementIsCorrect_ExpressionDefault()
        {
            var defaultValueConstraint = new DefaultValueConstraint();
            defaultValueConstraint.DefaultValue = @"datetime('now')";
            string output = defaultValueConstraint.CreateStatement();
            Assert.AreEqual(output, "DEFAULT (datetime('now'))");
        }
    }
}
