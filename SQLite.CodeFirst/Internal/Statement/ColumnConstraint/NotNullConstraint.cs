namespace SQLite.CodeFirst.Statement.ColumnConstraint
{
    internal class NotNullConstraint : IColumnConstraint
    {
        public string CreateStatement()
        {
            return "NOT NULL";
        }
    }
}
