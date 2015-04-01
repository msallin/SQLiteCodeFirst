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
            ConfigureTeamEntity(modelBuilder);
            ConfigureStadionEntity(modelBuilder);
            ConfigurePlayerEntity(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            var initializer = new FootballDbInitializer(Database.Connection.ConnectionString, modelBuilder);
            Database.SetInitializer(initializer);
        }

        private static void ConfigureTeamEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>();
        }

        private static void ConfigureStadionEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stadion>();
        }

        private static void ConfigurePlayerEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>()
                .HasRequired(p => p.Team)
                .WithMany(team => team.Players)
                .WillCascadeOnDelete(true);
        }
    }

    public class FootballDbInitializer : SqliteDropCreateDatabaseAlwaysInitializerBase<FootballDbContext>
    {
        public FootballDbInitializer(string connectionString, DbModelBuilder modelBuilder)
            : base(connectionString, modelBuilder) { }

        protected override void Seed(FootballDbContext context)
        {
            context.Set<Team>().Add(new Team
            {
                Name = "YB",
                Players = new List<Player>
                {
                    new Player
                    {
                        City = "Bern",
                        FirstName = "Marco",
                        LastName = "Bürki",
                        Street = "Wunderstrasse 43"
                    },
                    new Player
                    {
                        City = "Berlin",
                        FirstName = "Alain",
                        LastName = "Rochat",
                        Street = "Wonderstreet 13"
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
