using System.Collections.Generic;

namespace SQLite.CodeFirst.Statement
{
    internal interface IStatementCollection : IStatement, ICollection<IStatement>
    {
    }
}
