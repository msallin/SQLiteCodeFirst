using System;

namespace SQLite.CodeFirst
{
    /// <summary>
    /// When SQLite compares two strings, it uses a collating sequence or collating function (two words for the same thing)
    /// to determine which string is greater or if the two strings are equal. SQLite has three built-in collating functions (see <see cref="CollationFunction"/>).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CollateAttribute : Attribute
    {
        public CollateAttribute()
        {
            Collation = CollationFunction.None;
        }

        public CollateAttribute(CollationFunction collation)
        {
            Collation = collation;
        }

        public CollationFunction Collation { get; private set; }
    }
}