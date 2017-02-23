using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Statement.ColumnConstraint;

namespace SQLite.CodeFirst.Test.Statement.ColumnConstraint
{
    [TestClass]
    public class AutoincrementConstraintTest
    {
        [TestMethod]
        public void CreateStatement_StatementIsCorrect()
        {
            var uniqueConstraint = new AutoincrementConstraint();
            string output = uniqueConstraint.CreateStatement();
            Assert.AreEqual(output, "PRIMARY KEY AUTOINCREMENT");
        }
    }
}
