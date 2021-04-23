﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SQLite.CodeFirst.Statement
{
    internal class CompositePrimaryKeyStatement : Collection<string>, IStatement
    {
        private const string Key_Template = "`{key}`";
        private const string Template = "PRIMARY KEY({primary-keys})";
        private const string PrimaryKeyColumnNameSeperator = ", ";

        public CompositePrimaryKeyStatement(IEnumerable<string> keyMembers)
        {
            foreach (var keyMember in keyMembers)
            {
                Add(Key_Template.Replace("{key}", keyMember));
            }
        }

        public string CreateStatement()
        {
            string primaryKeys = String.Join(PrimaryKeyColumnNameSeperator, this);
            return Template.Replace("{primary-keys}", primaryKeys);
        }
    }
}
