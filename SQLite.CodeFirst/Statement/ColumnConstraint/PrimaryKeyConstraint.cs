namespace SQLite.CodeFirst.Statement.ColumnConstraint
{
    internal class PrimaryKeyConstraint : IColumnConstraint
    {
        public string CreateStatement()
        {
            return "PRIMARY KEY";
        }
    }
}
