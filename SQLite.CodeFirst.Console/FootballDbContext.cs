using System.Data.Entity;

namespace SQLite.CodeFirst.Console
{
    public class FootballDbContext : DbContext
    {
        public FootballDbContext()
            : base("footballDb")
        {
            Configuration.ProxyCreationEnabled = true;
            Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            ModelConfiguration.Configure(modelBuilder);
            var initializer = new FootballDbInitializer(modelBuilder);
            Database.SetInitializer(initializer);
        }
    }
}