using SQLite.CodeFirst.Statement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLite.CodeFirst.Statement.ColumnConstraint
{
    internal class AutoincrementConstraint: IColumnConstraint
    {
        public string CreateStatement()
        {
            return "PRIMARY KEY AUTOINCREMENT";
        }
    }
}
