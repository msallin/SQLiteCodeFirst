namespace SQLite.CodeFirst
{
    public class CollationData : ICollationData
    {
        public CollationFunction Collation { get; set; }

        public string Function { get; set; }
    }
}
