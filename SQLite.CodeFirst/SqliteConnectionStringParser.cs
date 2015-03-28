using System;
﻿using System.Collections.Generic;

namespace SQLite.CodeFirst
{
    internal static class SqliteConnectionStringParser
    {
        private const string DataDirectoryToken= "|DataDirectory|";
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
                keyValuePairDictionary.Add(keyValue[KeyPosition].ToLower(), keyValue[ValuePosition]);
            }

            return keyValuePairDictionary;
        }

        public static string GetDataSource(string connectionString)
        {
            if (connectionString.ToLower().Contains(DataDirectoryToken.ToLower()))
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\";
                connectionString = connectionString.ToLower().Replace(DataDirectoryToken.ToLower(), baseDirectory).Replace(@"\\", @"\");
            }
            return ParseSqliteConnectionString(connectionString)["data source"];
        }
    }
}
