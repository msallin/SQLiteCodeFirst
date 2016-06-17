using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace SQLite.CodeFirst.Console.Entity
{
    /// <summary>
    /// I agree, that this makes no sense.
    /// Its just to test the IsSelfReferencing method for table names which are shorter than 4 chars.
    /// See #66.
    /// </summary>
    public class Set
    {
        [Key]
        public string Title { get; set; }

        public virtual Player Player { get; set; }
    }
}
