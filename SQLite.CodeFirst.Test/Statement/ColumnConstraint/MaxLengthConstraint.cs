using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Statement.ColumnConstraint;

namespace SQLite.CodeFirst.Test.Statement.ColumnConstraint
{
    [TestClass]
    public class MaxLengthConstraintTest : StatementTestBase
    {
        [TestMethod]
        public void CreateStatementTest()
        {
            var maxLengthConstraint = new MaxLengthConstraint(12);
            string output = maxLengthConstraint.CreateStatement();
            Assert.AreEqual(output, "(12)");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateStatementInvalidParameterTest()
        {
            var maxLengthConstraint = new MaxLengthConstraint();
            maxLengthConstraint.CreateStatement();
        }
    }
}
