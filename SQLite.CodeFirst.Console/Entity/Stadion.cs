using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SQLite.CodeFirst.Console.Entity
{
    public class Stadion
    {
        [Key]
        [Column(Order = 1)]
        public string Name { get; set; }

        [Key]
        [Column(Order = 2)]
        public string Street { get; set; }

        [Key]
        [Column(Order = 3)]
        public string City { get; set; }
    }
}
