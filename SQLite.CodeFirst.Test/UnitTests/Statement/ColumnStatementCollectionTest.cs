using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst.Test.UnitTests.Statement
{
    [TestClass]
    public class ColumnStatementCollectionTest : StatementTestBase
    {
        [TestMethod]
        public void CreateStatementOneEntryTest()
        {
            var columnStatementCollection = new ColumnStatementCollection(new[] { CreateStatementMock("dummy1").Object });

            string output = columnStatementCollection.CreateStatement();
            Assert.AreEqual(1, columnStatementCollection.Count);
            Assert.AreEqual("dummy1", output);
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
            Assert.AreEqual(2, createIndexStatementCollection.Count);
            Assert.AreEqual("dummy1, dummy2", output);
        }
    }
}
