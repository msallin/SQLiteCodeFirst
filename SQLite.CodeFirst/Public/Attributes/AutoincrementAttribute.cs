using System;

namespace SQLite.CodeFirst
{
    /// <summary>
    /// Decorate an primary key with this attribute to create a "INTEGER PRIMARY KEY AUTOINCREMENT" column.
    /// <remarks>
    /// 1. The AUTOINCREMENT keyword imposes extra CPU, memory, disk space, and disk I/O overhead and should be avoided if not
    /// strictly needed. It is usually not needed.
    /// 2. In SQLite, a column with type INTEGER PRIMARY KEY is an alias for the ROWID(except in WITHOUT ROWID tables) which
    /// is always a 64-bit signed integer.
    /// 3. On an INSERT, if the ROWID or INTEGER PRIMARY KEY column is not explicitly given a value, then it will be filled
    /// automatically with an unused integer, usually one more than the largest ROWID currently in use.This is true
    /// regardless of whether or not the AUTOINCREMENT keyword is used.
    /// 4. If the AUTOINCREMENT keyword appears after INTEGER PRIMARY KEY, that changes the automatic ROWID assignment
    /// algorithm to prevent the reuse of ROWIDs over the lifetime of the database.In other words, the purpose of
    /// AUTOINCREMENT is to prevent the reuse of ROWIDs from previously deleted rows.
    /// http://sqlite.org/autoinc.html [24.02.2017]
    /// </remarks>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AutoincrementAttribute : Attribute
    {}
}