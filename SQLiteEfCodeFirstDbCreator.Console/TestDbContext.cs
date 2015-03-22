using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using SQLiteEfCodeFirstDbCreator.Console.Entity;

namespace SQLiteEfCodeFirstDbCreator.Console
{
    public class TestDbContext : DbContext
    {
        public TestDbContext()
            : base("test") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            ConfigureTeamEntity(modelBuilder);
            ConfigurePlayerEntity(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            var sqliteConnectionInitializer = new SqliteContextInitializer<TestDbContext>(
                Database.Connection.ConnectionString, modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);
        }

        private static void ConfigureTeamEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.RegisterEntityType(typeof(Team));
        }

        private static void ConfigurePlayerEntity(DbModelBuilder modelBuilder)
        {
            modelBuilder.RegisterEntityType(typeof(Player));

            modelBuilder.Entity<Player>().HasKey(player => player.Id);
            modelBuilder.Entity<Player>().Property(player => player.Name).HasMaxLength(10);

            modelBuilder.Entity<Player>()
                .HasRequired(p => p.Team)
                .WithMany(team => team.Players)
                .WillCascadeOnDelete(true);
        }
    }
}
