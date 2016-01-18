using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst.Builder
{
    internal interface IStatementBuilder<out TStatement>
        where TStatement : IStatement
    {
        TStatement BuildStatement();
    }
}
