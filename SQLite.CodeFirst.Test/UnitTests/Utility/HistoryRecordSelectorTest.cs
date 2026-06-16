using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Utility;

namespace SQLite.CodeFirst.Test.UnitTests.Utility
{
    [TestClass]
    public class HistoryRecordSelectorTest
    {
        private static IHistory Record(string context)
        {
            return new History { Context = context, Hash = "hash-" + context };
        }

        [TestMethod]
        public void SelectForContext_ReturnsRecordForGivenContext()
        {
            var records = new List<IHistory> { Record("ContextA"), Record("ContextB") };

            IHistory result = HistoryRecordSelector.SelectForContext(records, "ContextB");

            Assert.IsNotNull(result);
            Assert.AreEqual("ContextB", result.Context);
        }

        [TestMethod]
        public void SelectForContext_ReturnsNull_WhenNoRecordMatches()
        {
            var records = new List<IHistory> { Record("ContextA") };

            IHistory result = HistoryRecordSelector.SelectForContext(records, "ContextB");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void SelectForContext_ReturnsNull_WhenEmpty()
        {
            IHistory result = HistoryRecordSelector.SelectForContext(new List<IHistory>(), "ContextA");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void SelectForContext_Throws_WhenMultipleRecordsShareContext()
        {
            // One record per context is an invariant maintained by SaveHistory. If it is ever
            // violated, surfacing it is better than silently picking an arbitrary record.
            var records = new List<IHistory> { Record("ContextA"), Record("ContextA") };

            Assert.ThrowsExactly<InvalidOperationException>(
                () => HistoryRecordSelector.SelectForContext(records, "ContextA"));
        }
    }
}
