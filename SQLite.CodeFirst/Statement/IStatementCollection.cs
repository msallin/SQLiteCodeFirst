using System.Collections.Generic;

namespace SQLite.CodeFirst.Statement
{
    interface IStatementCollection : IStatement, ICollection<IStatement>
    {
    }
}
