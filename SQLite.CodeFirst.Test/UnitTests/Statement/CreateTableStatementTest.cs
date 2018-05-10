using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst.Test.UnitTests.Statement
{
    [TestClass]
    public class CreateTableStatementTest : StatementTestBase
    {
        [TestMethod]
        public void CreateStatementTest()
        {
            var createTableStatement = new CreateTableStatement
            {
                TableName = "dummyTable",
                ColumnStatementCollection = CreateStatementCollectionMock("dummyColumnDefinition").Object
            };

            string output = createTableStatement.CreateStatement();
            Assert.AreEqual(output, "CREATE TABLE dummyTable (dummyColumnDefinition);");
        }
    }
}
