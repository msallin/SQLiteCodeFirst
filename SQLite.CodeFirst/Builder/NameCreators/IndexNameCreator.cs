using System;
using System.Globalization;

namespace SQLite.CodeFirst.Builder.NameCreators
{
    internal static class IndexNameCreator
    {
        public const string IndexNamePrefix = "IX_";

        public static string CreateIndexName(string tableName, string propertyName)
        {
            // If the tableName is escaped it means that this name contains special chars e.g. a dot (base.myTable)
            // Because the tablename is used to build the index name the index name must also be escaped.
            bool escaped = tableName.StartsWith(SpecialChars.EscapeCharOpen.ToString()) &&
                           tableName.EndsWith(SpecialChars.EscapeCharClose.ToString());

            tableName = tableName.Trim(SpecialChars.EscapeCharOpen, SpecialChars.EscapeCharClose);

            string prefixChar = String.Empty;
            string postfixChar = String.Empty;
            if (escaped)
            {
                prefixChar = SpecialChars.EscapeCharOpen.ToString();
                postfixChar = SpecialChars.EscapeCharClose.ToString();
            }

            return String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}_{3}{4}", prefixChar, IndexNamePrefix, tableName, propertyName, postfixChar);
        }
    }
}
