using System;

namespace SQLite.CodeFirst
{
    /// <summary>
    /// This class can be used to specify the default collation for the database. Explicit Collate attributes will take precendence.
    /// When SQLite compares two strings, it uses a collating sequence or collating function (two words for the same thing)
    /// to determine which string is greater or if the two strings are equal. SQLite has three built-in collating functions (see <see cref="Function"/>).
    /// Set  <see cref="Function"/> to <see cref="CollationFunction.Custom"/> and specify the name using the function parameter.
    /// </summary>
    public class Collation
    {
        public Collation()
            : this(CollationFunction.None)
        {
        }

        public Collation(CollationFunction function)
            : this(function, null)
        {
        }

        public Collation(CollationFunction function, string customFunction)
        {
            if (function != CollationFunction.Custom && !string.IsNullOrEmpty(customFunction))
            {
                throw new ArgumentException("If the collation is not set to CollationFunction.Custom a function must not be specified.", nameof(function));
            }

            if (function == CollationFunction.Custom && string.IsNullOrEmpty(customFunction))
            {
                throw new ArgumentException("If the collation is set to CollationFunction.Custom a function must be specified.", nameof(function));
            }

            CustomFunction  = customFunction;
            Function = function;
        }

        public CollationFunction Function { get; set; }

        /// <summary>
        /// The name of the custom collating function to use (CollationFunction.Custom).
        /// </summary>
        public string CustomFunction  { get; set; }
    }
}
