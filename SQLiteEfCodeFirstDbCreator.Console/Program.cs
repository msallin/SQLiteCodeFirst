using System.Linq;
using SQLiteEfCodeFirstDbCreator.Console.Entity;

namespace SQLiteEfCodeFirstDbCreator.Console
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
