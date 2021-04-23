using System.Globalization;

namespace SQLite.CodeFirst.Builder.NameCreators
{
    internal static class ColumnNameCreator
    {
        public static string EscapeName(string columnName)
        {
            return string.Format(CultureInfo.InvariantCulture, "[{0}]", columnName);
        }
    }
}