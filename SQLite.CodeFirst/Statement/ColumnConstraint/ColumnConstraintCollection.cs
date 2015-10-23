using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SQLite.CodeFirst.Statement.ColumnConstraint
{
    internal class ColumnConstraintCollection : Collection<IColumnConstraint>, IColumnConstraintCollection
    {
        private const string ConstraintStatementSeperator = " ";

        public ColumnConstraintCollection()
            : this(new List<IColumnConstraint>())
        { }

        public ColumnConstraintCollection(IEnumerable<IColumnConstraint> columnConstraints)
        {
            foreach (var columnConstraint in columnConstraints)
            {
                Add(columnConstraint);
            }
        }

        public string CreateStatement()
        {
            return String.Join(ConstraintStatementSeperator, this.Select(c => c.CreateStatement()));
        }
    }
}
