using System.Text;

namespace SQLite.CodeFirst.Statement.ColumnConstraint
{
    internal class CollateConstraint : IColumnConstraint
    {
        private const string Template = "COLLATE {collation-name}";

        public CollationFunction CollationFunction { get; set; }

        public string CustomCollationFunction { get; set; }

        public string CreateStatement()
        {
            if (CollationFunction == CollationFunction.None)
            {
                return string.Empty;
            }

            var sb = new StringBuilder(Template);

            string name = CollationFunction == CollationFunction.Custom ? CustomCollationFunction : CollationFunction.ToString().ToUpperInvariant();
            sb.Replace("{collation-name}", name);
            
            return sb.ToString().Trim();
        }
    }
}
