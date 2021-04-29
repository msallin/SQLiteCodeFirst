namespace SQLite.CodeFirst
{
    public interface ICollationData
    {
        CollationFunction Collation { get; }

        string Function { get; }
    }
}