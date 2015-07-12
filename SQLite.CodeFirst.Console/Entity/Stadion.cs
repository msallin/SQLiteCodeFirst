using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SQLite.CodeFirst.Console.Entity
{
    public class Stadion
    {
        [Key]
        [Column(Order = 1)]
        [Index("IX_Stadion_Main", Order = 2)] // Test for combined, named index
        public string Name { get; set; }

        [Key]
        [Column(Order = 2)]
        [Index("IX_Stadion_Main", Order = 1)] // Test for combined, named index
        public string Street { get; set; }

        [Key]
        [Column(Order = 3)]
        public string City { get; set; }
    }
}
