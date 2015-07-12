using System.ComponentModel.DataAnnotations;

namespace SQLite.CodeFirst.Console.Entity
{
    public abstract class Person : IEntity
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }
    }
}
