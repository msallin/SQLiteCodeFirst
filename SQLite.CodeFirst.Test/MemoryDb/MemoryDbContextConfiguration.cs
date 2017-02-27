using System;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite.EF6;
using System.Reflection;

namespace SQLite.CodeFirst.Test.MemoryDb
{
    public class MemoryDbContextConfiguration : DbConfiguration
    {
        public MemoryDbContextConfiguration()
        {
            this.SetDefaultConnectionFactory(new SQLiteConnectionFactory());
            //this.SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            this.SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
            Type t = Type.GetType("System.Data.SQLite.EF6.SQLiteProviderServices, System.Data.SQLite.EF6");
            FieldInfo fi = t.GetField("Instance", BindingFlags.NonPublic | BindingFlags.Static);
            SetProviderServices("System.Data.SQLite", (DbProviderServices)fi.GetValue(null));
        }
    }
}
