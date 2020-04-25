using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.Linq;

namespace SQLite.CodeFirst.Console
{
    public class Configuration : DbConfiguration
    {
        public Configuration()
        {
            AddDependencyResolver(SQLiteDependencyResolver.Instance);
        }

        private class SQLiteDependencyResolver : IDbDependencyResolver
        {
            public static IDbDependencyResolver Instance { get; }
                = new SQLiteDependencyResolver();

            public object GetService(Type type, object key)
            {
                return type == typeof(DbProviderFactory)
                    ? SQLiteProviderFactory.Instance
                    : type == typeof(IDbProviderFactoryResolver)
                    ? SQLiteDbProviderFactoryResolver.Instance
                    : type == typeof(IProviderInvariantName)
                    ? SQLiteProviderInvariantName.Instance
                    : SQLiteProviderFactory.Instance.GetService(type);
            }

            public IEnumerable<object> GetServices(Type type, object key)
                => Enumerable.Empty<object>();
        }

        private class SQLiteDbProviderFactoryResolver : IDbProviderFactoryResolver
        {
            public static IDbProviderFactoryResolver Instance { get; }
                = new SQLiteDbProviderFactoryResolver();

            public DbProviderFactory ResolveProviderFactory(DbConnection connection)
            {
                return connection switch
                {
                    SQLiteConnection _ => SQLiteProviderFactory.Instance,
                    EntityConnection _ => EntityProviderFactory.Instance,
                    _ => null
                };
            }
        }

        private class SQLiteProviderInvariantName : IProviderInvariantName
        {
            public static IProviderInvariantName Instance { get; }
                = new SQLiteProviderInvariantName();

            public string Name { get; } = "System.Data.SQLite.EF6";
        }
    }
}
