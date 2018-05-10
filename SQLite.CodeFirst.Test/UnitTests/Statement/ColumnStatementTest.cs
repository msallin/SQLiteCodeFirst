using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SQLite.CodeFirst.Statement;
using SQLite.CodeFirst.Statement.ColumnConstraint;

namespace SQLite.CodeFirst.Test.UnitTests.Statement
{
    [TestClass]
    public class ColumnStatementTest : StatementTestBase
    {
        [TestMethod]
        public void CreateStatement()
        {
            var columnConstraintsMock = new Mock<IColumnConstraintCollection>();
            columnConstraintsMock.Setup(c => c.CreateStatement()).Returns("dummyColumnConstraint");

            var columnStatement = new ColumnStatement
            {
                ColumnConstraints = columnConstraintsMock.Object,
                ColumnName = "dummyColumnName",
                TypeName = "dummyType"
            };
            string output = columnStatement.CreateStatement();
            Assert.AreEqual(output, "[dummyColumnName] dummyType dummyColumnConstraint");
        }
    }
}
