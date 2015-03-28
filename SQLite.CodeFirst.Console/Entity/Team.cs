using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SQLite.CodeFirst.Console.Entity
{
    public class Team : IEntity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Player> Players { get; set; }

        public virtual Stadion Stadion { get; set; }
    }
}