using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Utility;

namespace SQLite.CodeFirst.Test.UnitTests.Utility
{
    [TestClass]
    public class ConnectionStringParserTest
    {
        [TestMethod]
        public void GetDataSource_ReturnsCorrectDataSource()
        {
            // Arrange
            string connectionString = @"data source=.\db\footballDb\footballDb.sqlite;foreign keys=true";


            // Act
            string result = ConnectionStringParser.GetDataSource(connectionString);

            // Assert
            Assert.AreEqual(@".\db\footballDb\footballDb.sqlite", result);
        }

        [TestMethod]
        public void GetDataSource_ReturnsCorrectDataSource_NotLowerCase()
        {
            // Arrange
            string connectionString = @"DatA SOurce=.\db\footballDb\footballDb.sqlite;foreign keys=true";


            // Act
            string result = ConnectionStringParser.GetDataSource(connectionString);

            // Assert
            Assert.AreEqual(@".\db\footballDb\footballDb.sqlite", result);
        }

        [TestMethod]
        public void GetDataSource_ReturnsCorrectDataSource_WithSpace()
        {
            // Arrange
            string connectionString = @"DatA SOurce =.\db\footballDb\footballDb.sqlite;foreign keys=true";


            // Act
            string result = ConnectionStringParser.GetDataSource(connectionString);

            // Assert
            Assert.AreEqual(@".\db\footballDb\footballDb.sqlite", result);
        }

        [TestMethod]
        public void GetDataSource_ReturnsCorrectDataSource_ReplacesDataDirectory()
        {
            // Arrange
            string connectionString = @"data source=|DataDirectory|\db\footballDb\footballDb.sqlite;foreign keys=true";


            // Act
            string result = ConnectionStringParser.GetDataSource(connectionString);

            // Assert
            Assert.IsTrue(!result.StartsWith("|DataDirectory|"));
            Assert.IsTrue(result.EndsWith(@"db\footballDb\footballDb.sqlite"));
        }

        [TestMethod]
        public void GetDataSource_ReturnsCorrectDataSource_ReplacesDataDirectoryCaseIsIgnored()
        {
            // Arrange
            string connectionString = @"data source=|dAtadIrectory|\db\footballDb\footballDb.sqlite;foreign keys=true";


            // Act
            string result = ConnectionStringParser.GetDataSource(connectionString);

            // Assert
            Assert.IsTrue(!result.StartsWith("|dAtadIrectory|"));
            Assert.IsTrue(result.EndsWith(@"\db\footballDb\footballDb.sqlite"));
        }

        [TestMethod]
        public void GetDataSource_ReturnsCorrectDataSource_ReplacesDataDirectoryAddsBackslash()
        {
            // Arrange
            string connectionString = @"data source=|DataDirectory|db\footballDb\footballDb.sqlite;foreign keys=true";


            // Act
            string result = ConnectionStringParser.GetDataSource(connectionString);

            // Assert
            Assert.IsTrue(!result.StartsWith("|DataDirectory|"));
            Assert.IsTrue(result.EndsWith(@"\db\footballDb\footballDb.sqlite"));
        }

        [TestMethod]
        public void GetDataSource_ReturnsCorrectDataSource_RemovesQuotationMark()
        {
            // Arrange
            string connectionString = @"data source="".\db\footballDb\footballDb.sqlite"";foreign keys=true";


            // Act
            string result = ConnectionStringParser.GetDataSource(connectionString);

            // Assert
            Assert.AreEqual(@".\db\footballDb\footballDb.sqlite", result);
        }
    }
}
