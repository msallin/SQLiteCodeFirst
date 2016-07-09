using System.Data.Entity;
using SQLite.CodeFirst.Console.Entity;

namespace SQLite.CodeFirst.Console
{
    public class ModelConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            ConfigureTeamEntity(modelBuilder);
            ConfigureStadionEntity(modelBuilder);
            ConfigureCoachEntity(modelBuilder);
            ConfigurePlayerEntity(modelBuilder);
            ConfigureSelfReferencingEntities(modelBuilder);
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

        private static void ConfigureSelfReferencingEntities(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Foo>();
            modelBuilder.Entity<FooSelf>();
            modelBuilder.Entity<FooStep>();
        }
    }
}
