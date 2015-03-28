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
        [MaxLength(100)]
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        public Team Team { get; set; }
    }
}