using System;

namespace SQLite.CodeFirst
{
    /// <summary>
    /// The UNIQUE Constraint prevents two records from having identical values in a particular column.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class UniqueAttribute : Attribute
    {
        public UniqueAttribute(OnConflictAction onConflict = OnConflictAction.None)
        {
            OnConflict = onConflict;
        }

        public OnConflictAction OnConflict { get; set; }
    }
}