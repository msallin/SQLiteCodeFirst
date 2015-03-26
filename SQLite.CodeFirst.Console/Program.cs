using System.Linq;
using SQLite.CodeFirst.Console.Entity;

namespace SQLite.CodeFirst.Console
{
    public static class Program
    {
        static void Main()
        {
            var context = new TestDbContext();
            System.Console.WriteLine(context.Set<Player>().Count());
            System.Console.ReadLine();
        }
    }
}
