using System.Text;

namespace SQLite.CodeFirst.Statement.ColumnConstraint
{
    internal class DefaultValueConstraint : IColumnConstraint
    {
        private const string Template = "DEFAULT ({defaultValue})";

        public string DefaultValue { get; set; }

        public string CreateStatement()
        {
            var sb = new StringBuilder(Template);

            sb.Replace("{defaultValue}", DefaultValue);

            return sb.ToString().Trim();
        }
    }
}
