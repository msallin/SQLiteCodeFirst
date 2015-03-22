using SQLiteEfCodeFirstDbCreator.Statement;

namespace SQLiteEfCodeFirstDbCreator.Builder
{
    public interface IStatementBuilder<out TStatement>
        where TStatement : IStatement
    {
        TStatement BuildStatement();
    }
}
