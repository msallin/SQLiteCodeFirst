using System.Collections.Generic;

namespace SQLite.CodeFirst.Statement
{
    public interface IStatementCollection : IStatement, ICollection<IStatement>
    {
    }
}
