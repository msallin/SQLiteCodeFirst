using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using System.Linq;

namespace SQLite.CodeFirst.MigrationConsole
{
    public class FootballDbContext : DbContext
    {
        public FootballDbContext()
        {
            Configure();
        }

        public FootballDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Configure();
        }

        public FootballDbContext(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection)
        {
            Configure();
        }

        private void Configure()
        {
            Configuration.ProxyCreationEnabled = true;
            Configuration.LazyLoadingEnabled = true;

            Database.Log += s => { System.Diagnostics.Debug.WriteLine(s); };
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            ModelConfiguration.Configure(modelBuilder);
            Database.SetInitializer(new SqliteMigrateDatabaseToLatestVersion<FootballDbContext, Migrations.Configuration>(modelBuilder, true));
        }
    }
}