using System.Data.Entity;
using SQLite.CodeFirst.Console.Entity;

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
            ConfigureTeamEntity(modelBuilder);
            ConfigureStadionEntity(modelBuilder);
            ConfigureCoachEntity(modelBuilder);
            ConfigurePlayerEntity(modelBuilder);

            var initializer = new FootballDbInitializer(modelBuilder);
            Database.SetInitializer(initializer);
        }

        private static void ConfigureTeamEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>().ToTable("Base.MyTable")
                .HasRequired(t => t.Coach)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Team>()
                .HasRequired(t => t.Stadion)
                .WithRequiredPrincipal()
                .WillCascadeOnDelete(true);
        }

        private static void ConfigureStadionEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stadion>();
        }

        private static void ConfigureCoachEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Coach>()
                .HasRequired(p => p.Team)
                .WithRequiredPrincipal(t => t.Coach)
                .WillCascadeOnDelete(false);
        }

        private static void ConfigurePlayerEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>()
                .HasRequired(p => p.Team)
                .WithMany(team => team.Players)
                .WillCascadeOnDelete(true);
        }
    }
}
