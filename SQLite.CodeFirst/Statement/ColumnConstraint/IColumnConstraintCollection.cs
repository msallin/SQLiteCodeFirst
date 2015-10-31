using System.Collections.Generic;

namespace SQLite.CodeFirst.Statement.ColumnConstraint
{
    interface IColumnConstraintCollection : ICollection<IColumnConstraint>, IColumnConstraint
    {
    }
}
