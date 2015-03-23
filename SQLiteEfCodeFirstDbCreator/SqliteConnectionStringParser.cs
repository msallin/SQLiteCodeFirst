using System.Collections.Generic;

namespace SQLiteEfCodeFirstDbCreator
{
    internal static class SqliteConnectionStringParser
    {
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
                keyValuePairDictionary.Add(keyValue[KeyPosition], keyValue[ValuePosition]);
            }

            return keyValuePairDictionary;
        }

        public static string GetDataSource(string connectionString)
        {
            return ParseSqliteConnectionString(connectionString)["data source"];
        }
    }
}
