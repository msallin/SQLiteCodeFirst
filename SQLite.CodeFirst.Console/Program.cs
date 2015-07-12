using SQLite.CodeFirst.Console.Entity;

namespace SQLite.CodeFirst.Console
{
    public static class Program
    {
        static void Main()
        {
            System.Console.WriteLine("Starting Demo Application");

            var context = CreateAndSeedDatabase();

            DisplaySeededData(context);

            PressEnterToExit();
        }

        private static FootballDbContext CreateAndSeedDatabase()
        {
            System.Console.WriteLine("Create and seed the database.");
            var context = new FootballDbContext();
            System.Console.WriteLine("Completed.");
            System.Console.WriteLine();
            return context;
        }

        private static void DisplaySeededData(FootballDbContext context)
        {
            System.Console.WriteLine("Display seeded data.");

            foreach (var team in context.Set<Team>())
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

                foreach (var player in team.Players)
                {
                    System.Console.WriteLine("\t\t Player:");
                    System.Console.WriteLine("\t\t Id: {0}", player.Id);
                    System.Console.WriteLine("\t\t Number: {0}", player.Number);
                    System.Console.WriteLine("\t\t FirstName: {0}", player.FirstName);
                    System.Console.WriteLine("\t\t LastName: {0}", player.LastName);
                    System.Console.WriteLine("\t\t Street: {0}", player.Street);
                    System.Console.WriteLine("\t\t City: {0}", player.City);
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
