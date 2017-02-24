namespace SQLite.CodeFirst.Statement.ColumnConstraint
{
    internal class AutoincrementConstraint : IColumnConstraint
    {
        public string CreateStatement()
        {
            return "PRIMARY KEY AUTOINCREMENT";
        }
    }
}
