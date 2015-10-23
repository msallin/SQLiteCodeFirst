using Moq;
using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst.Test.Statement
{
    public abstract class StatementTestBase
    {
        protected Mock<IStatement> CreateStatementMock(string createStatementReturnValue)
        {
            var statementMock = new Mock<IStatement>();
            statementMock.Setup(s => s.CreateStatement()).Returns(createStatementReturnValue);
            return statementMock;
        }
    }
}
