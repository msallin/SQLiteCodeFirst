namespace SQLite.CodeFirst.MigrationConsole.Entity
{
    /// <summary>
    /// See https://github.com/msallin/SQLiteCodeFirst/issues/69 and https://github.com/msallin/SQLiteCodeFirst/issues/63
    /// </summary>
    public class FooSelf
    {
        public int FooSelfId { get; set; }
        public int FooId { get; set; }
        public int Number { get; set; }
        public virtual Foo Foo { get; set; }
    }
}