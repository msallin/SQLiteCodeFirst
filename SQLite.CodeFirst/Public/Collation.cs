namespace SQLite.CodeFirst
{
    /// <summary>
    /// This class can be used to specify the default collation for the database. Explicit Collate attributes will take precendence.
    /// When SQLite compares two strings, it uses a collating sequence or collating function (two words for the same thing)
    /// to determine which string is greater or if the two strings are equal. SQLite has three built-in collating functions (see <see cref="CollationFunction"/>).
    /// Set  <see cref="CollationFunction"/> to <see cref="CollationFunction.Custom"/> and specify the name using the function parameter.
    /// </summary>
    public class Collation
    {
        public CollationFunction CollationFunction { get; set; }

        /// <summary>
        /// The name of the custom collating function to use (CollationFunction.Custom).
        /// </summary>
        public string Function { get; set; }
    }
}
