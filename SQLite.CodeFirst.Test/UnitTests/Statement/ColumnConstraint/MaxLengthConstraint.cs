using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Statement.ColumnConstraint;

namespace SQLite.CodeFirst.Test.UnitTests.Statement.ColumnConstraint
{
    [TestClass]
    public class MaxLengthConstraintTest : StatementTestBase
    {
        [TestMethod]
        public void CreateStatementTest()
        {
            var maxLengthConstraint = new MaxLengthConstraint(12);
            string output = maxLengthConstraint.CreateStatement();
            Assert.AreEqual("(12)", output);
        }

        [TestMethod]
        public void CreateStatementInvalidParameterTest()
        {
            var maxLengthConstraint = new MaxLengthConstraint();
            Assert.ThrowsExactly<InvalidOperationException>(() => maxLengthConstraint.CreateStatement());
        }
    }
}
