using Moq;
using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst.Test.UnitTests.Statement
{
    public abstract class StatementTestBase
    {
        protected Mock<IStatement> CreateStatementMock(string createStatementReturnValue)
        {
            var statementMock = new Mock<IStatement>();
            statementMock.Setup(s => s.CreateStatement()).Returns(createStatementReturnValue);
            return statementMock;
        }

        protected Mock<IStatementCollection> CreateStatementCollectionMock(string createStatementReturnValue)
        {
            var statementCollectionMock = new Mock<IStatementCollection>();
            statementCollectionMock.Setup(s => s.CreateStatement()).Returns(createStatementReturnValue);
            return statementCollectionMock;
        }
    }
}
