using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Utility;

namespace SQLite.CodeFirst.Test.Utility
{
    [TestClass]
    public class HistoryEntityTypeValidatorTest
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EnsureValidTypeNotIHistory()
        {
            HistoryEntityTypeValidator.EnsureValidType(typeof(InvalidFakeHistoryType1));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EnsureValidTypeNoParamLessCtorTest()
        {
            HistoryEntityTypeValidator.EnsureValidType(typeof(InvalidFakeHistoryType2));
        }

        [TestMethod]
        public void EnsureValidTypeTest()
        {
            HistoryEntityTypeValidator.EnsureValidType(typeof(ValidFakeHistoryType));
        }

        private class ValidFakeHistoryType : IHistory
        {
            public int Id { get; set; }
            public string Hash { get; set; }
            public string Context { get; set; }
            public DateTime CreateDate { get; set; }
        }

        private class InvalidFakeHistoryType1
        {
            public int Id { get; set; }
            public string Hash { get; set; }
            public string Context { get; set; }
            public DateTime CreateDate { get; set; }
        }

        private class InvalidFakeHistoryType2 : IHistory
        {
            public InvalidFakeHistoryType2(string test)
            { }

            public int Id { get; set; }
            public string Hash { get; set; }
            public string Context { get; set; }
            public DateTime CreateDate { get; set; }
        }
    }
}
