namespace SQLite.CodeFirst.Console.Entity
{
    public class Player : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Team Team { get; set; }
    }
}