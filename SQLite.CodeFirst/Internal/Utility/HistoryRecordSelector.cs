using System.Collections.Generic;
using System.Linq;

namespace SQLite.CodeFirst.Utility
{
    /// <summary>
    /// Selects the history record that belongs to a specific context from the history table.
    /// A database can be shared by multiple contexts, so the lookup must be scoped by the context
    /// key. Selecting without that scope would return more than one record on a shared database and
    /// make the underlying <see cref="Enumerable.SingleOrDefault{TSource}(IEnumerable{TSource})"/> throw.
    /// </summary>
    internal static class HistoryRecordSelector
    {
        public static IHistory SelectForContext(IEnumerable<IHistory> records, string contextKey)
        {
            return records.SingleOrDefault(record => record.Context == contextKey);
        }
    }
}
