using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Utility;

namespace SQLite.CodeFirst.Test.UnitTests.Utility
{
    [TestClass]
    public class HashCreatorTest
    {
        [TestMethod]
        public void CreateHashTest()
        {
            string result = HashCreator.CreateHash("Test,Test,Test");
            Assert.AreEqual("kMbs8GbjyafvacMkuACV+tDtaoM9ii8y7pxi8AjgfcFincyIrDiD6R8kTiO5lupnmcYqZMUHtQk144aV3HTTCg==", result);
        }

        [TestMethod]
        public void CreateHashNotSameHashTest()
        {
            string result = HashCreator.CreateHash("Test,Test,Test!");
            Assert.AreNotEqual("kMbs8GbjyafvacMkuACV+tDtaoM9ii8y7pxi8AjgfcFincyIrDiD6R8kTiO5lupnmcYqZMUHtQk144aV3HTTCg==", result);
        }
    }
}
