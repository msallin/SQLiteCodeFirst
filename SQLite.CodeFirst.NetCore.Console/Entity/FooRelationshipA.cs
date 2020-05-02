using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SQLite.CodeFirst.NetCore.Console.Entity
{
    /// <summary>
    /// See https://github.com/msallin/SQLiteCodeFirst/issues/109
    /// </summary>
    public class FooRelationshipA
    {
        public int Id { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public virtual ICollection<FooCompositeKey> Fooey { get; set; }
    }
}