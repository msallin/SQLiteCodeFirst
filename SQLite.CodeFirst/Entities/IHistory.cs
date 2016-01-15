using System;

namespace SQLite.CodeFirst
{
    public interface IHistory
    {
        int Id { get; set; }
        string Hash { get; set; }
        string Context { get; set; }
        DateTime CreateDate { get; set; }
    }
}