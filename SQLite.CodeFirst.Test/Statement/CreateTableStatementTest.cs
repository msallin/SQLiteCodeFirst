using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst.Test.Statement
{
    [TestClass]
    public class CreateTableStatementTest : StatementTestBase
    {
        [TestMethod]
        public void CreateStatementTest()
        {
            var statementCollectionMock = new Mock<IStatementCollection>();
            statementCollectionMock.Setup(s => s.CreateStatement()).Returns("dummyColumnDefinition");

            var createTableStatement = new CreateTableStatement
            {
                TableName = "dummyTable",
                ColumnStatementCollection = statementCollectionMock.Object
            };

            string output = createTableStatement.CreateStatement();
            Assert.AreEqual(output, "CREATE TABLE dummyTable (dummyColumnDefinition);");
        }
    }
}
