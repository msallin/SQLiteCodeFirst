using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace SQLite.CodeFirst.Utility
{
    internal static class ConnectionStringParser
    {
        private const string DataDirectoryToken = "|datadirectory|";
        private const string DataSourceToken = "data source";
        private const char KeyValuePairSeperator = ';';
        private const char KeyValueSeperator = '=';
        private const int KeyPosition = 0;
        private const int ValuePosition = 1;

        public static string GetDataSource(string connectionString)
        {
            // If the datasource token does not exists this is a FullUri connection string.
            IDictionary<string, string> strings = ParseConnectionString(connectionString);
            if (strings.ContainsKey(DataSourceToken))
            {
                var path = ExpandDataDirectory(ParseConnectionString(connectionString)[DataSourceToken]);
                return path.Trim('"');
            }

            // TODO: Implement FullUri parsing.
            if (connectionString.Contains(":memory:")) 
            {
                return ":memory:";
            }
            throw new NotSupportedException("FullUri format is currently only supported for :memory:.");
        }

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "ToUppercase makes no sense.")]
        private static IDictionary<string, string> ParseConnectionString(string connectionString)
        {
            connectionString = connectionString.Trim();
            string[] keyValuePairs = connectionString.Split(KeyValuePairSeperator);

            IDictionary<string, string> keyValuePairDictionary = new Dictionary<string, string>();
            foreach (var keyValuePair in keyValuePairs)
            {
                string[] keyValue = keyValuePair.Split(KeyValueSeperator);
                if (keyValue.Length >= 2)
                {
                    keyValuePairDictionary.Add(keyValue[KeyPosition].Trim().ToLower(CultureInfo.InvariantCulture), keyValue[ValuePosition]);
                }
            }

            return keyValuePairDictionary;
        }

        private static string ExpandDataDirectory(string path)
        {
            if (path == null || !path.StartsWith(DataDirectoryToken, StringComparison.OrdinalIgnoreCase))
            {
                return path;
            }

            string fullPath;

            // find the replacement path
            object rootFolderObject = AppDomain.CurrentDomain.GetData("DataDirectory");
            string rootFolderPath = rootFolderObject as string;
            if (rootFolderObject != null && rootFolderPath == null)
            {
                throw new InvalidOperationException("The value stored in the AppDomains 'DataDirectory' variable has to be a string!");
            }
            if (string.IsNullOrEmpty(rootFolderPath))
            {
                rootFolderPath = AppDomain.CurrentDomain.BaseDirectory;
            }

            // We don't know if rootFolderpath ends with '\', and we don't know if the given name starts with onw
            int fileNamePosition = DataDirectoryToken.Length;    // filename starts right after the '|datadirectory|' keyword
            bool rootFolderEndsWith = (0 < rootFolderPath.Length) && rootFolderPath[rootFolderPath.Length - 1] == Path.DirectorySeparatorChar;
            bool fileNameStartsWith = (fileNamePosition < path.Length) && path[fileNamePosition] == Path.DirectorySeparatorChar;

            // replace |datadirectory| with root folder path
            if (!rootFolderEndsWith && !fileNameStartsWith)
            {
                // need to insert '\'
                fullPath = rootFolderPath + Path.DirectorySeparatorChar + path.Substring(fileNamePosition);
            }
            else if (rootFolderEndsWith && fileNameStartsWith)
            {
                // need to strip one out
                fullPath = rootFolderPath + path.Substring(fileNamePosition + 1);
            }
            else
            {
                // simply concatenate the strings
                fullPath = rootFolderPath + path.Substring(fileNamePosition);
            }
            return fullPath;
        }
    }
}
