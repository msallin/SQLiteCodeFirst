using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace SQLite.CodeFirst.Utility
{
    internal static class HashCreator
    {
        public static string CreateHash(string data)
        {
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hashBytes = sha512.ComputeHash(dataBytes);
                string hash = Convert.ToBase64String(hashBytes);
                return hash;
            }
        }
    }
}
