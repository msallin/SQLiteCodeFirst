using System;
using System.Globalization;

namespace SQLite.CodeFirst.Builder.NameCreators
{
    internal static class TableNameCreator
    {
        public static string CreateTableName(string entityName)
        {
            return CreateTableName(null, entityName);
        }

        public static string CreateTableName(string schema, string entityName)
        {
            if (!String.IsNullOrWhiteSpace(schema) && schema != "dbo")
            {
                return String.Format(CultureInfo.InvariantCulture, "{0}{1}.{2}{3}", SpecialChars.EscapeCharOpen, schema, entityName, SpecialChars.EscapeCharClose);
            }

            return entityName;
        }
    }
}
