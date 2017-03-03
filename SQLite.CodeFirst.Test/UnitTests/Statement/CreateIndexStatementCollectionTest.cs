using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst.Test.UnitTests.Statement
{
    [TestClass]
    public class CreateIndexStatementCollectionTest : StatementTestBase
    {
        [TestMethod]
        public void CreateStatementOneEntryTest()
        {
            var createIndexStatementCollection = new CreateIndexStatementCollection(new[]
            {
                CreateStatementMock("dummy1").Object,
            });

            string output = createIndexStatementCollection.CreateStatement();
            Assert.AreEqual(createIndexStatementCollection.Count, 1);
            Assert.AreEqual(output, "dummy1");
        }

        [TestMethod]
        public void CreateStatementTwoEntryTest()
        {
            var createIndexStatementCollection = new CreateIndexStatementCollection(new[]
            {
                CreateStatementMock("dummy1").Object,
                CreateStatementMock("dummy2").Object
            });

            string output = createIndexStatementCollection.CreateStatement();
            Assert.AreEqual(createIndexStatementCollection.Count, 2);
            Assert.AreEqual(output, "dummy1\r\ndummy2");
        }
    }
}
