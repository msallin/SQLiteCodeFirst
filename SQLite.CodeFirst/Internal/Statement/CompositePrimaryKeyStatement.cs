using SQLite.CodeFirst.Builder.NameCreators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SQLite.CodeFirst.Statement
{
    internal class CompositePrimaryKeyStatement : Collection<string>, IStatement
    {
        private const string Template = "PRIMARY KEY({primary-keys})";

        public CompositePrimaryKeyStatement(IEnumerable<string> keyMembers)
        {
            foreach (var keyMember in keyMembers)
            {
                Add(keyMember);
            }
        }

        public string CreateStatement()
        {
            string primaryKeys = String.Join(", ", this.Select(c => ColumnNameCreator.EscapeName(c)));
            return Template.Replace("{primary-keys}", primaryKeys);
        }
    }
}
