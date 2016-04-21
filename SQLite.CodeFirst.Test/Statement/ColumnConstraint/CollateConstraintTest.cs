using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Statement.ColumnConstraint;

namespace SQLite.CodeFirst.Test.Statement.ColumnConstraint
{
    [TestClass]
    public class CollateConstraintTest
    {
        [TestMethod]
        public void CreateStatement_StatementIsCorrect_NoConstraint()
        {
            var collationConstraint = new CollateConstraint();
            collationConstraint.CollationFunction = CollationFunction.None;
            string output = collationConstraint.CreateStatement();
            Assert.AreEqual(output, "");
        }

        [TestMethod]
        public void CreateStatement_StatementIsCorrect_NoCase()
        {
            var collationConstraint = new CollateConstraint();
            collationConstraint.CollationFunction = CollationFunction.NoCase;
            string output = collationConstraint.CreateStatement();
            Assert.AreEqual(output, "COLLATE NOCASE");
        }
    }
}
