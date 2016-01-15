using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst.Builder
{
    public interface IStatementBuilder<out TStatement>
        where TStatement : IStatement
    {
        TStatement BuildStatement();
    }
}
