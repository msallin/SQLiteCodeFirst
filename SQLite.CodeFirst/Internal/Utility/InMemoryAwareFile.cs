using System;
using System.IO;

namespace SQLite.CodeFirst.Utility
{
    internal class InMemoryAwareFile
    {
        public static bool Exists(string path)
        {
            if (IsInMemoryPath(path))
            {
                return false;
            }
            return File.Exists(path);
        }

        public static bool Exists(string path, bool nullByteFileMeansNotExisting)
        {
            if (IsInMemoryPath(path))
            {
                return false;
            }

            var fileInfo = new FileInfo(path);
            return nullByteFileMeansNotExisting ? fileInfo.Exists && fileInfo.Length != 0 : fileInfo.Exists;
        }

        public static void Delete(string path)
        {
            if (IsInMemoryPath(path))
            {
                return;
            }

            File.Delete(path);
        }

        public static void CreateDirectory(string path)
        {
            if (IsInMemoryPath(path))
            {
                return;
            }

            var dbFileInfo = new FileInfo(path);
            if (dbFileInfo.Directory != null)
            {
                dbFileInfo.Directory.Create();
            }
        }


        private static bool IsInMemoryPath(string path)
        {
            return string.Equals(path, ":memory:", StringComparison.OrdinalIgnoreCase);
        }
    }
}