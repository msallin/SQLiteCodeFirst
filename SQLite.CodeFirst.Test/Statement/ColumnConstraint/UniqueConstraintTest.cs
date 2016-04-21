using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Statement.ColumnConstraint;

namespace SQLite.CodeFirst.Test.Statement.ColumnConstraint
{
    [TestClass]
    public class UniqueConstraintTest
    {
        [TestMethod]
        public void CreateStatement_StatementIsCorrect_NoConstraint()
        {
            var uniqueConstraint = new UniqueConstraint();
            uniqueConstraint.OnConflict = OnConflictAction.None;
            string output = uniqueConstraint.CreateStatement();
            Assert.AreEqual(output, "UNIQUE");
        }

        [TestMethod]
        public void CreateStatement_StatementIsCorrect_WithConstraint()
        {
            var uniqueConstraint = new UniqueConstraint();
            uniqueConstraint.OnConflict = OnConflictAction.Rollback;
            string output = uniqueConstraint.CreateStatement();
            Assert.AreEqual(output, "UNIQUE ON CONFLICT ROLLBACK");
        }
    }
}
