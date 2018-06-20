namespace SQLite.CodeFirst.MigrationConsole.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<SQLite.CodeFirst.MigrationConsole.FootballDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;

            // This command alter the class to support Migration to SQLite. 
            SetSqlGenerator("System.Data.SQLite", new SqliteMigrationSqlGenerator());
        }

        protected override void Seed(SQLite.CodeFirst.MigrationConsole.FootballDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
