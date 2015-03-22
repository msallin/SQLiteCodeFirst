using System.Collections.Generic;

namespace SQLiteEfCodeFirstDbCreator.Console.Entity
{
    public class Team : IEntity
    {
        public int Id { get; set; }
        public ICollection<Player> Players { get; set; }
    }
}