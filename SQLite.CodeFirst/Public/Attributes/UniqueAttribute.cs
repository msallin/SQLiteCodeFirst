using System;

namespace SQLite.CodeFirst
{
    /// <summary>
    /// The UNIQUE Constraint prevents two records from having identical values in a particular column.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class UniqueAttribute : Attribute
    {
        public UniqueAttribute()
        {
            OnConflict = OnConflictAction.None;
        }

        public UniqueAttribute(OnConflictAction onConflict)
        {
            OnConflict = onConflict;
        }

        public OnConflictAction OnConflict { get; private set; }
    }
}