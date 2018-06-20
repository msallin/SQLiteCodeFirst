using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SQLite.CodeFirst.MigrationConsole.Entity
{
    public class Team : IEntity
    {
        [Autoincrement]
        public int Id { get; set; }

        [Index("IX_Team_TeamsName")] // Test for named index.
        [Required]
        public string Name { get; set; }

        public virtual Coach Coach { get; set; }

        public virtual ICollection<Player> Players { get; set; }

        public virtual Stadion Stadion { get; set; }
    }
}