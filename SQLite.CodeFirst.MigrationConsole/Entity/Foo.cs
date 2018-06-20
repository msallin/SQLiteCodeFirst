using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SQLite.CodeFirst.MigrationConsole.Entity
{
    /// <summary>
    /// See https://github.com/msallin/SQLiteCodeFirst/issues/69 and https://github.com/msallin/SQLiteCodeFirst/issues/63
    /// </summary>
    public class Foo
    {
        private ICollection<FooSelf> _fooSelves;
        private ICollection<FooStep> _fooSteps;

        public int FooId { get; set; }
        public string Name { get; set; }
        public int? FooSelf1Id { get; set; }
        public int? FooSelf2Id { get; set; }
        public int? FooSelf3Id { get; set; }

        [ForeignKey("FooSelf1Id")]
        public virtual Foo ParentMyEntity1 { get; set; }

        [ForeignKey("FooSelf2Id")]
        public virtual Foo ParentMyEntity2 { get; set; }

        [ForeignKey("FooSelf3Id")]
        public virtual Foo ParentMyEntity3 { get; set; }

        public virtual ICollection<FooStep> FooSteps
        {
            get { return _fooSteps ?? (_fooSteps = new HashSet<FooStep>()); }
            set { _fooSteps = value; }
        }

        public virtual ICollection<FooSelf> FooSelves
        {
            get { return _fooSelves ?? (_fooSelves = new HashSet<FooSelf>()); }
            set { _fooSelves = value; }
        }
    }
}