using System;
using System.Globalization;

namespace SQLite.CodeFirst.Builder.NameCreators
{
    internal static class IndexNameCreator
    {
        public const string IndexNamePrefix = "IX_";

        public static string CreateName(string tableName, string propertyName)
        {
            // If the tableName is escaped it means that this name contains special chars e.g. a dot (base.myTable)
            // Because the tablename is used to build the index name the index name must also be escaped.
            tableName = tableName.Trim(SpecialChars.EscapeCharOpen, SpecialChars.EscapeCharClose);
            return String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}_{3}{4}", SpecialChars.EscapeCharOpen, IndexNamePrefix, tableName, propertyName, SpecialChars.EscapeCharClose);
        }
    }
}