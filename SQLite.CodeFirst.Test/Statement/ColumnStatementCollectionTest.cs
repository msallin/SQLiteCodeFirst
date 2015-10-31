using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst.Test.Statement
{
    [TestClass]
    public class ColumnStatementCollectionTest : StatementTestBase
    {
        [TestMethod]
        public void CreateStatementOneEntryTest()
        {
            var columnStatementCollection = new ColumnStatementCollection(new[] { CreateStatementMock("dummy1").Object });

            string output = columnStatementCollection.CreateStatement();
            Assert.AreEqual(columnStatementCollection.Count, 1);
            Assert.AreEqual(output, "dummy1");
        }

        [TestMethod]
        public void CreateStatementTwoEntryTest()
        {
            var createIndexStatementCollection = new ColumnStatementCollection(new[]
            {
                CreateStatementMock("dummy1").Object,
                CreateStatementMock("dummy2").Object
            });

            string output = createIndexStatementCollection.CreateStatement();
            Assert.AreEqual(createIndexStatementCollection.Count, 2);
            Assert.AreEqual(output, "dummy1, dummy2");
        }
    }
}
