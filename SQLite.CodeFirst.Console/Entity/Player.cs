using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SQLite.CodeFirst.Console.Entity
{
    public class Player
    {
        [Key]
        [Column(Order = 1)] 
        public string FirstName { get; set; }
        [Key]
        [Column(Order = 2)] 
        public string LastName { get; set; }
        public Team Team { get; set; }
    }
}