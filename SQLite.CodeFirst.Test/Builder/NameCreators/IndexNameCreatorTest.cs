using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Builder.NameCreators;

namespace SQLite.CodeFirst.Test.Builder.NameCreators
{
    [TestClass]
    public class IndexNameCreatorTest
    {
        [TestMethod]
        public void CreateIndexName()
        {
            string result = IndexNameCreator.CreateIndexName("MyTable", "MyProperty");
            Assert.AreEqual("\"IX_MyTable_MyProperty\"", result);
        }

        [TestMethod]
        public void CreateIndexNameEscaped()
        {
            string result = IndexNameCreator.CreateIndexName("\"base.MyTable\"", "MyProperty");
            Assert.AreEqual("\"IX_base.MyTable_MyProperty\"", result);
        }
    }
}
