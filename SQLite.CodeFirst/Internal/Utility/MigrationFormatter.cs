using System;
using System.CodeDom.Compiler;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Migrations.Model;
using System.Globalization;
using System.IO;

namespace SQLite.CodeFirst.Utility
{
    public static class MigrationFormatter
    {
        const int _defaultStringMaxLength = 255;
        const int _defaultNumericPrecision = 10;
        const byte _defaultTimePrecision = 7;
        const byte _defaultNumericScale = 0;
        
        /// <summary>
        /// Builds a column type
        /// </summary>
        /// <returns> SQL representing the data type. </returns>
        public static string BuildColumnType(DbProviderManifest providerManifest, ColumnModel column)
        {
            if (column == null)
                return string.Empty;

            return column.IsTimestamp ? "rowversion" : BuildPropertyType(providerManifest, column);
        }

        /// <summary>
        /// Builds a SQL property type fragment from the specified <see cref="ColumnModel"/>.
        /// </summary>
        public static string BuildPropertyType(DbProviderManifest providerManifest, PropertyModel column)
        {
            if (providerManifest == null || column == null)
                return string.Empty;

            var originalStoreType = column.StoreType;

            if (string.IsNullOrWhiteSpace(originalStoreType))
            {
                var typeUsage = providerManifest.GetStoreType(column.TypeUsage).EdmType;
                originalStoreType = typeUsage.Name;
            }

            var storeType = originalStoreType;

            const string maxSuffix = "(max)";

            if (storeType.EndsWith(maxSuffix, StringComparison.Ordinal))
                storeType = storeType.Substring(0, storeType.Length - maxSuffix.Length) + maxSuffix;

            switch (originalStoreType.ToUpperInvariant())
            {
                case "DECIMAL":
                case "NUMERIC":
                    storeType += "(" + (column.Precision ?? _defaultNumericPrecision)
                                     + ", " + (column.Scale ?? _defaultNumericScale) + ")";
                    break;
                case "DATETIME":
                case "TIME":
                    storeType += "(" + (column.Precision ?? _defaultTimePrecision) + ")";
                    break;
                case "BLOB":
                case "VARCHAR2":
                case "VARCHAR":
                case "CHAR":
                case "NVARCHAR":
                case "NVARCHAR2":
                    storeType += "(" + (column.MaxLength ?? _defaultStringMaxLength) + ")";
                    break;
            }

            return storeType;
        }

        /// <summary>
        /// Gets an <see cref="IndentedTextWriter" /> object used to format SQL script.
        /// </summary>
        /// <returns> An empty text writer to use for SQL generation. </returns>
        public static IndentedTextWriter CreateIndentedTextWriter()
        {
            var writer = new StringWriter(CultureInfo.InvariantCulture);
            try
            {
                return new IndentedTextWriter(writer);
            }
            catch
            {
                writer.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Remove occurrences of "dbo." from the supplied string.
        /// </summary>
        public static string RemoveDbo(string str)
        {
            if (str == null)
                return string.Empty;

            return str.Replace("dbo.", string.Empty);
        }

        /// <summary>
        /// Surround with double-quotes Sqlite reserved words.
        /// </summary>
        public static string ReservedWord(string word)
        {
            if (word == null)
                return string.Empty;

            switch (word.ToUpper(CultureInfo.InvariantCulture))
            {
                case "ABORT":
                case "ACTION":
                case "ADD":
                case "AFTER":
                case "ALL":
                case "ALTER":
                case "ANALYZE":
                case "AND":
                case "AS":
                case "ASC":
                case "ATTACH":
                case "AUTOINCREMENT":
                case "BEFORE":
                case "BEGIN":
                case "BETWEEN":
                case "BY":
                case "CASCADE":
                case "CASE":
                case "CAST":
                case "CHECK":
                case "COLLATE":
                case "COLUMN":
                case "COMMIT":
                case "CONFLICT":
                case "CONSTRAINT":
                case "CREATE":
                case "CROSS":
                case "CURRENT_DATE":
                case "CURRENT_TIME":
                case "CURRENT_TIMESTAMP":
                case "DATABASE":
                case "DEFAULT":
                case "DEFERRABLE":
                case "DEFERRED":
                case "DELETE":
                case "DESC":
                case "DETACH":
                case "DISTINCT":
                case "DROP":
                case "EACH":
                case "ELSE":
                case "END":
                case "ESCAPE":
                case "EXCEPT":
                case "EXCLUSIVE":
                case "EXISTS":
                case "EXPLAIN":
                case "FAIL":
                case "FOR":
                case "FOREIGN":
                case "FROM":
                case "FULL":
                case "GLOB":
                case "GROUP":
                case "HAVING":
                case "IF":
                case "IGNORE":
                case "IMMEDIATE":
                case "IN":
                case "INDEX":
                case "INDEXED":
                case "INITIALLY":
                case "INNER":
                case "INSERT":
                case "INSTEAD":
                case "INTERSECT":
                case "INTO":
                case "IS":
                case "ISNULL":
                case "JOIN":
                case "KEY":
                case "LEFT":
                case "LIKE":
                case "LIMIT":
                case "MATCH":
                case "NATURAL":
                case "NO":
                case "NOT":
                case "NOTNULL":
                case "NULL":
                case "OF":
                case "OFFSET":
                case "ON":
                case "OR":
                case "ORDER":
                case "OUTER":
                case "PLAN":
                case "PRAGMA":
                case "PRIMARY":
                case "QUERY":
                case "RAISE":
                case "RECURSIVE":
                case "REFERENCES":
                case "REGEXP":
                case "REINDEX":
                case "RELEASE":
                case "RENAME":
                case "REPLACE":
                case "RESTRICT":
                case "RIGHT":
                case "ROLLBACK":
                case "ROW":
                case "SAVEPOINT":
                case "SELECT":
                case "SET":
                case "TABLE":
                case "TEMP":
                case "TEMPORARY":
                case "THEN":
                case "TO":
                case "TRANSACTION":
                case "TRIGGER":
                case "UNION":
                case "UNIQUE":
                case "UPDATE":
                case "USING":
                case "VACUUM":
                case "VALUES":
                case "VIEW":
                case "VIRTUAL":
                case "WHEN":
                case "WHERE":
                case "WITH":
                case "WITHOUT":
                    return '"' + word + '"';

                default:
                    return word;
            }
        }

        public static string UniqueConflictText(AnnotationValues uniqueAnnotation)
        {
            if (uniqueAnnotation == null)
                return string.Empty;

            var uniqueText = Convert.ToString(uniqueAnnotation.NewValue, CultureInfo.InvariantCulture);
            OnConflictAction action;

            if (!uniqueText.StartsWith("OnConflict:", StringComparison.OrdinalIgnoreCase))
                return string.Empty;

            var actionText = uniqueText.Remove(0, "OnConflict:".Length).Trim();
            if (!Enum.TryParse(actionText, out action))
                return string.Empty;

            if (action == OnConflictAction.None)
                return string.Empty;

            return " ON CONFLICT " + action.ToString().ToUpperInvariant();
        }

        public static string CollateFunctionText(AnnotationValues collateAnnotation)
        {
            if (collateAnnotation == null)
                return string.Empty;

            var collateAttributeText = Convert.ToString(collateAnnotation.NewValue, CultureInfo.InvariantCulture);
            string collateFunction;
            string collateCustomFunction;

            if (collateAttributeText.IndexOf(':') > -1)
            {
                collateFunction = collateAttributeText.Substring(0, collateAttributeText.IndexOf(':'));
                collateCustomFunction = collateAttributeText.Remove(0, collateAttributeText.IndexOf(':') + 1).Trim();
            }
            else
            {
                collateFunction = collateAttributeText;
                collateCustomFunction = string.Empty;
            }

            CollationFunction colatteFunctionType;
            if (!Enum.TryParse(collateFunction, out colatteFunctionType))
                return string.Empty;

            if (colatteFunctionType == CollationFunction.None)
            {
                return string.Empty;
            }

            return colatteFunctionType == CollationFunction.Custom
                ? " COLLATE " + collateCustomFunction
                : " COLLATE " + colatteFunctionType.ToString().ToUpperInvariant();
        }

    }
}
