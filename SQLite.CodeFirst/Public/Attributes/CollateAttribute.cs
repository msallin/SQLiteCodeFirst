using System;

namespace SQLite.CodeFirst
{
    /// <summary>
    /// When SQLite compares two strings, it uses a collating sequence or collating function (two words for the same thing)
    /// to determine which string is greater or if the two strings are equal. SQLite has three built-in collating functions (see <see cref="CollationFunction"/>).
    /// It is possible to specify a custom collating function. Set  <see cref="CollationFunction"/> to <see cref="CollationFunction.Custom"/> and specify the name using the function parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CollateAttribute : Attribute
    {
        public CollateAttribute()
        {
            Collation = new Collation();
        }

        public CollateAttribute(CollationFunction function)
        {
            Collation = new Collation(function);
        }

        public CollateAttribute(CollationFunction function, string customFunction)
        {
            Collation = new Collation(function, customFunction);
        }

        public Collation Collation { get; }
    }
}