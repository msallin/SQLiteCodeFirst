using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SQLite.CodeFirst.MigrationConsole.Entity
{
	/// <summary>
	/// See https://github.com/msallin/SQLiteCodeFirst/issues/109
	/// </summary>
	public class FooCompositeKey
	{
		[Key, Column(Order = 1)]
		public int Id { get; set; }

		[Key, Column(Order = 2), StringLength(20)]
		public string Version { get; set; }

		[StringLength(255)]
		public string Name { get; set; }

		public virtual ICollection<FooRelationshipA> FooeyACollection { get; set; }

		public virtual ICollection<FooRelationshipB> FooeyBCollection { get; set; }
	}
}