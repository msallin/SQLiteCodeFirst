using System.ComponentModel.DataAnnotations.Schema;

namespace SQLite.CodeFirst.MigrationConsole.Entity
{
    [Table("TeamPlayer")]
    public class Player : Person
    {
        [Index] // Automatically named 'IX_TeamPlayer_Number'
        [Index("IX_TeamPlayer_NumberPerTeam", Order = 1, IsUnique = true)]
        public int Number { get; set; }

        // The index attribute must be placed on the FK not on the navigation property (team).
        [Index("IX_TeamPlayer_NumberPerTeam", Order = 2, IsUnique = true)]
        public int TeamId { get; set; }

        // Its not possible to set an index on this property. Use the FK property (teamId).
        public virtual Team Team { get; set; }

        public virtual Player Mentor { get; set; }
    }
}