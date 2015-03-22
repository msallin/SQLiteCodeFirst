using System;
using System.Collections.Generic;
using System.Linq;

namespace SQLiteEfCodeFirstDbCreator.Statement.ColumnConstraint
{
    internal class ColumnConstraintCollection : IColumnConstraint
    {
        private const string ConstraintStatementSeperator = " ";

        public ICollection<IColumnConstraint> ColumnConstraints { get; set; }

        public string CreateStatement()
        {
            return String.Join(ConstraintStatementSeperator, ColumnConstraints.Select(c => c.CreateStatement()));
        }
    }
}
