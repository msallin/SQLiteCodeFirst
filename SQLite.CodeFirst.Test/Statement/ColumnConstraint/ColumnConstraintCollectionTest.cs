using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SQLite.CodeFirst.Statement.ColumnConstraint;

namespace SQLite.CodeFirst.Test.Statement.ColumnConstraint
{
    [TestClass]
    public class ColumnConstraintCollectionTest : StatementTestBase
    {
        [TestMethod]
        public void CreateStatementOneColumnConstraintTest()
        {
            var columnConstraintMock = new Mock<IColumnConstraint>();
            columnConstraintMock.Setup(c => c.CreateStatement()).Returns("dummy1");

            var columnConstraintCollection = new ColumnConstraintCollection(new[]
            {
                columnConstraintMock.Object
            });
            string output = columnConstraintCollection.CreateStatement();
            Assert.AreEqual(output, "dummy1");
        }

        [TestMethod]
        public void CreateStatementTwoColumnConstraintsTest()
        {
            var columnConstraintMock1 = new Mock<IColumnConstraint>();
            columnConstraintMock1.Setup(c => c.CreateStatement()).Returns("dummy1");

            var columnConstraintMock2 = new Mock<IColumnConstraint>();
            columnConstraintMock2.Setup(c => c.CreateStatement()).Returns("dummy2");

            var columnConstraintCollection = new ColumnConstraintCollection(new[]
            {
                columnConstraintMock1.Object,
                columnConstraintMock2.Object
            });
            string output = columnConstraintCollection.CreateStatement();
            Assert.AreEqual(output, "dummy1 dummy2");
        }
    }
}
