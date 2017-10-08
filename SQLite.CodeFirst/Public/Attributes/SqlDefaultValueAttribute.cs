using System;

namespace SQLite.CodeFirst
{
    /// <summary>
    /// Decorate an column with this attribute to create a "DEFAULT {defaultvalue}".
    /// <remarks>
    /// https://www.sqlite.org/lang_createtable.html [05.10.2017]
    /// </remarks>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SqlDefaultValueAttribute : Attribute
    {
        public string DefaultValue { get; set; }
    }
}