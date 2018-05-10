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
            Collation = CollationFunction.None;
        }

        public CollateAttribute(CollationFunction collation)
        {
            if (collation == CollationFunction.Custom)
            {
                throw new ArgumentException("If the collation is set to CollationFunction.Custom a function must be specified.", nameof(collation));
            }

            Collation = collation;
        }
        public CollateAttribute(CollationFunction collation, string function)
        {
            if (collation != CollationFunction.Custom && !string.IsNullOrEmpty(function))
            {
                throw new ArgumentException("If the collation is not set to CollationFunction.Custom a function must not be specified.", nameof(function));
            }

            if (collation == CollationFunction.Custom && string.IsNullOrEmpty(function))
            {
                throw new ArgumentException("If the collation is set to CollationFunction.Custom a function must be specified.", nameof(function));
            }

            Collation = collation;
            Function = function;
        }

        public CollationFunction Collation { get; }

        /// <summary>
        /// The name of the custom collating function to use (CollationFunction.Custom).
        /// </summary>
        public string Function { get; }
    }
}