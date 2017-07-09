using System.Globalization;

namespace SQLite.CodeFirst.Builder.NameCreators
{
    internal static class NameCreator
    {
        public static string EscapeName(string name)
        {
            // Ensure that the name is not already escaped.
            name = name.Trim(SpecialChars.EscapeCharOpen, SpecialChars.EscapeCharClose);

            // Escape the escape chars, if there are some of them in the name.
            name = name.Replace(SpecialChars.EscapeCharOpen.ToString(), SpecialChars.EscapeCharOpen.ToString() + SpecialChars.EscapeCharOpen);

            // Escape the name.
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", SpecialChars.EscapeCharOpen, name, SpecialChars.EscapeCharClose);
        }
    }
}