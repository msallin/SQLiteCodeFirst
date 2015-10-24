using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
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
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

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

    public class FootballDbInitializer : SqliteDropCreateDatabaseAlways<FootballDbContext>
    {
        public FootballDbInitializer(DbModelBuilder modelBuilder)
            : base(modelBuilder)
        { }

        protected override void Seed(FootballDbContext context)
        {
            context.Set<Team>().Add(new Team
            {
                Name = "YB",
                Coach = new Coach
                {
                    City = "Zürich",
                    FirstName = "Masssaman",
                    LastName = "Nachn",
                    Street = "Testingstreet 844"
                },
                Players = new List<Player>
                {
                    new Player
                    {
                        City = "Bern",
                        FirstName = "Marco",
                        LastName = "Bürki",
                        Street = "Wunderstrasse 43",
                        Number = 12
                    },
                    new Player
                    {
                        City = "Berlin",
                        FirstName = "Alain",
                        LastName = "Rochat",
                        Street = "Wonderstreet 13",
                        Number = 14
                    }
                },
                Stadion = new Stadion
                {
                    Name = "Stade de Suisse",
                    City = "Bern",
                    Street = "Papiermühlestrasse 71"
                }
            });
        }
    }
}
