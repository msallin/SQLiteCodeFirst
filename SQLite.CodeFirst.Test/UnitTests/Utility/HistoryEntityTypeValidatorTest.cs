using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Utility;

namespace SQLite.CodeFirst.Test.UnitTests.Utility
{
    [TestClass]
    public class HistoryEntityTypeValidatorTest
    {
        [TestMethod]
        public void EnsureValidTypeNotIHistory()
        {
            Assert.ThrowsExactly<InvalidOperationException>(
                () => HistoryEntityTypeValidator.EnsureValidType(typeof(InvalidFakeHistoryType1)));
        }

        [TestMethod]
        public void EnsureValidTypeNoParamLessCtorTest()
        {
            Assert.ThrowsExactly<InvalidOperationException>(
                () => HistoryEntityTypeValidator.EnsureValidType(typeof(InvalidFakeHistoryType2)));
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
