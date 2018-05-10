using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using SQLite.CodeFirst.Console.Entity;

namespace SQLite.CodeFirst.Console
{
    public static class Program
    {
        private static void Main()
        {
            StartDemoUseInMemory();
            StartDemoUseFile();
            PressEnterToExit();
        }

        private static void StartDemoUseInMemory()
        {
            System.Console.WriteLine("Starting Demo Application (In Memory)");
            System.Console.WriteLine(string.Empty);

            using (var sqLiteConnection = new SQLiteConnection("data source=:memory:"))
            {
                // This is required if a in memory db is used.
                sqLiteConnection.Open();

                using (var context = new FootballDbContext(sqLiteConnection, false))
                {
                    CreateAndSeedDatabase(context);
                    DisplaySeededData(context);
                }
            }
        }

        private static void StartDemoUseFile()
        {
            System.Console.WriteLine("Starting Demo Application (File)");
            System.Console.WriteLine(string.Empty);

            using (var context = new FootballDbContext("footballDb"))
            {
                CreateAndSeedDatabase(context);
                DisplaySeededData(context);
            }
        }

        private static void CreateAndSeedDatabase(DbContext context)
        {
            System.Console.WriteLine("Create and seed the database.");

            if (context.Set<Team>().Count() != 0)
            {
                return;
            }

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

            context.SaveChanges();

            System.Console.WriteLine("Completed.");
            System.Console.WriteLine();
        }

        private static void DisplaySeededData(DbContext context)
        {
            System.Console.WriteLine("Display seeded data.");

            foreach (Team team in context.Set<Team>())
            {
                System.Console.WriteLine("\t Team:");
                System.Console.WriteLine("\t Id: {0}", team.Id);
                System.Console.WriteLine("\t Name: {0}", team.Name);
                System.Console.WriteLine();

                System.Console.WriteLine("\t\t Stadion:");
                System.Console.WriteLine("\t\t Name: {0}", team.Stadion.Name);
                System.Console.WriteLine("\t\t Street: {0}", team.Stadion.Street);
                System.Console.WriteLine("\t\t City: {0}", team.Stadion.City);
                System.Console.WriteLine();

                System.Console.WriteLine("\t\t Coach:");
                System.Console.WriteLine("\t\t Id: {0}", team.Coach.Id);
                System.Console.WriteLine("\t\t FirstName: {0}", team.Coach.FirstName);
                System.Console.WriteLine("\t\t LastName: {0}", team.Coach.LastName);
                System.Console.WriteLine("\t\t Street: {0}", team.Coach.Street);
                System.Console.WriteLine("\t\t City: {0}", team.Coach.City);
                System.Console.WriteLine();

                foreach (Player player in team.Players)
                {
                    System.Console.WriteLine("\t\t Player:");
                    System.Console.WriteLine("\t\t Id: {0}", player.Id);
                    System.Console.WriteLine("\t\t Number: {0}", player.Number);
                    System.Console.WriteLine("\t\t FirstName: {0}", player.FirstName);
                    System.Console.WriteLine("\t\t LastName: {0}", player.LastName);
                    System.Console.WriteLine("\t\t Street: {0}", player.Street);
                    System.Console.WriteLine("\t\t City: {0}", player.City);
                    System.Console.WriteLine("\t\t Created: {0}", player.CreatedUtc);
                    System.Console.WriteLine();
                }
            }
        }

        private static void PressEnterToExit()
        {
            System.Console.WriteLine();
            System.Console.WriteLine("Press 'Enter' to exit.");
            System.Console.ReadLine();
        }
    }
}