using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SQLite.CodeFirst.Console.Entity
{
    public abstract class Person : IEntity
    {
        public int Id { get; set; }

        [MaxLength(50)]
        [Collate(CollationFunction.NoCase)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [SqlDefaultValue(DefaultValue = "DATETIME('now')")]
        public DateTime CreatedUtc { get; set; }
    }
}
