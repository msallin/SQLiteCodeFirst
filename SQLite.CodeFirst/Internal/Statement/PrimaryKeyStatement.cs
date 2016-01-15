using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SQLite.CodeFirst.Statement
{
    internal class PrimaryKeyStatement : Collection<string>, IStatement
    {
        private const string Template = "PRIMARY KEY({primary-keys})";
        private const string PrimaryKeyColumnNameSeperator = ", ";

        public PrimaryKeyStatement(IEnumerable<string> keyMembers)
        {
            foreach (var keyMember in keyMembers)
            {
                Add(keyMember);
            }
        }

        public string CreateStatement()
        {
            string primaryKeys = String.Join(PrimaryKeyColumnNameSeperator, this);
            return Template.Replace("{primary-keys}", primaryKeys);
        }
    }
}
