using System;

namespace SQLite.CodeFirst.Statement.ColumnConstraint
{
    internal class MaxLengthConstraint : IColumnConstraint
    {
        private const string Template = "({max-length})";

        public int? MaxLength { get; set; }

        public MaxLengthConstraint() { }

        public MaxLengthConstraint(int maxLength)
        {
            MaxLength = maxLength;
        }

        public string CreateStatement()
        {
            if (MaxLength == null)
            {
                throw new InvalidOperationException("MaxLength must not be null!");
            }

            return Template.Replace("{max-length}", MaxLength.ToString());
        }
    }
}
