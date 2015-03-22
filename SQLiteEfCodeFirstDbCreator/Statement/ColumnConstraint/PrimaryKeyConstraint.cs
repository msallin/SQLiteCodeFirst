namespace SQLiteEfCodeFirstDbCreator.Statement.ColumnConstraint
{
    internal class PrimaryKeyConstraint : IColumnConstraint
    {
        public string CreateStatement()
        {
            return "PRIMARY KEY";
        }
    }
}
