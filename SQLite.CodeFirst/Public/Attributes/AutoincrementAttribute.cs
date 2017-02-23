using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLite.CodeFirst
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AutoincrementAttribute: Attribute
    {
    }
}
