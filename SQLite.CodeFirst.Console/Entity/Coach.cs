namespace SQLite.CodeFirst.Console.Entity
{
    public class Coach : Person
    {
        public virtual Team Team { get; set; }
    }
}
