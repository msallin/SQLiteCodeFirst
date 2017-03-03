using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Builder.NameCreators;

namespace SQLite.CodeFirst.Test.UnitTests.Builder.NameCreators
{
    [TestClass]
    public class TableNameCreatorTest
    {
        [TestMethod]
        public void CreateTableNameTest()
        {
            string result = TableNameCreator.CreateTableName("Test");
            Assert.AreEqual("\"Test\"", result);
        }
    }
}
