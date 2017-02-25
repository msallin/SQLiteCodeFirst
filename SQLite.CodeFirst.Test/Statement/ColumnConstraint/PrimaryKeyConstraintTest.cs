using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Statement.ColumnConstraint;

namespace SQLite.CodeFirst.Test.Statement.ColumnConstraint
{
    [TestClass]
    public class PrimaryKeyConstraintTest
    {
        [TestMethod]
        public void CreateStatement_StatementIsCorrect_WithAutoincrement()
        {
            var primaryKeyConstraint = new PrimaryKeyConstraint { Autoincrement = true };
            string output = primaryKeyConstraint.CreateStatement();
            Assert.AreEqual(output, "PRIMARY KEY AUTOINCREMENT");
        }

        [TestMethod]
        public void CreateStatement_StatementIsCorrect()
        {
            var primaryKeyConstraint = new PrimaryKeyConstraint();
            string output = primaryKeyConstraint.CreateStatement();
            Assert.AreEqual(output, "PRIMARY KEY");
        }
    }
}
