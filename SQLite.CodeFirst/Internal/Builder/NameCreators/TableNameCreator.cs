using System;
using System.Globalization;

namespace SQLite.CodeFirst.Builder.NameCreators
{
    internal static class TableNameCreator
    {
        public static string CreateTableName(string tableFromEntitySet)
        {
            tableFromEntitySet = tableFromEntitySet.Trim(SpecialChars.EscapeCharOpen, SpecialChars.EscapeCharClose);
            return String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", SpecialChars.EscapeCharOpen, tableFromEntitySet, SpecialChars.EscapeCharClose);
        }
    }
}
