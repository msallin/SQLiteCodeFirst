namespace SQLite.CodeFirst.MigrationConsole.Entity
{
    public class Coach : Person
    {
        public virtual Team Team { get; set; }
    }
}
