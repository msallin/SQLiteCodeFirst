namespace SQLite.CodeFirst.NetCore.Console.Entity
{
    public class Coach : Person
    {
        public virtual Team Team { get; set; }
    }
}
