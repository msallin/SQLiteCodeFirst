namespace SQLite.CodeFirst
{
    /// <summary>
    /// The action to resolve a UNIQUE constraint violation.
    /// Is used together with the <see cref="UniqueAttribute"/>.
    /// </summary>
    public enum OnConflictAction
    {
        None,
        Rollback,
        Abort,
        Fail,
        Ignore,
        Replace
    }
}