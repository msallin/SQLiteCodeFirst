using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Builder.NameCreators;

namespace SQLite.CodeFirst.Test.UnitTests.Builder.NameCreators
{
    [TestClass]
    public class NameCreatorTest
    {
        [TestMethod]
        public void CreateTableNameTest()
        {
            string result = NameCreator.EscapeName("Test");
            Assert.AreEqual("\"Test\"", result);
        }
    }
}
