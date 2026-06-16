using System;

namespace SQLite.CodeFirst
{
    /// <summary>
    /// Adds 'ON UPDATE CASCADE' to the foreign key constraint of the relationship the decorated property belongs to.
    /// Entity Framework has no concept of update-cascade (it assumes primary keys are immutable), so neither the
    /// <see cref="System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute"/> nor the fluent API can express it.
    /// This opt-in attribute is the only way to emit the keyword.
    /// Place it on the dependent foreign key property (e.g. the 'TeamId' property), not on the navigation property (e.g. 'Team').
    /// Requires an explicit foreign key property; relationships with a shadow foreign key column cannot be decorated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CascadeOnUpdateAttribute : Attribute
    {
        public CascadeOnUpdateAttribute()
        {
            CanCascade = true;
        }

        public CascadeOnUpdateAttribute(bool canCascade)
        {
            CanCascade = canCascade;
        }

        public bool CanCascade { get; }
    }
}
