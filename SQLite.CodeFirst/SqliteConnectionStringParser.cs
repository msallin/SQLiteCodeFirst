using System;
using System.Collections.Generic;

namespace SQLite.CodeFirst
{
    internal static class SqliteConnectionStringParser
    {
        private const string DataDirectoryToken = "|datadirectory|";
        private const char KeyValuePairSeperator = ';';
        private const char KeyValueSeperator = '=';
        private const int KeyPosition = 0;
        private const int ValuePosition = 1;

        public static IDictionary<string, string> ParseSqliteConnectionString(string connectionString)
        {
            connectionString = connectionString.Trim();
            string[] keyValuePairs = connectionString.Split(KeyValuePairSeperator);

            IDictionary<string, string> keyValuePairDictionary = new Dictionary<string, string>();
            foreach (var keyValuePair in keyValuePairs)
            {
                string[] keyValue = keyValuePair.Split(KeyValueSeperator);
                if (keyValue.Length >= 2){
                    keyValuePairDictionary.Add(keyValue[KeyPosition].ToLower(), keyValue[ValuePosition]);
                }
            }

            return keyValuePairDictionary;
        }

        public static string GetDataSource(string connectionString)
        {
            var path = ExpandDataDirectory(ParseSqliteConnectionString(connectionString)["data source"]);
            return path;
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
            string rootFolderPath = (rootFolderObject as string);
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
            bool rootFolderEndsWith = (0 < rootFolderPath.Length) && rootFolderPath[rootFolderPath.Length - 1] == '\\';
            bool fileNameStartsWith = (fileNamePosition < path.Length) && path[fileNamePosition] == '\\';

            // replace |datadirectory| with root folder path
            if (!rootFolderEndsWith && !fileNameStartsWith)
            {
                // need to insert '\'
                fullPath = rootFolderPath + '\\' + path.Substring(fileNamePosition);
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
