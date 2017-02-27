using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Console;
using SQLite.CodeFirst.Console.Entity;

namespace SQLite.CodeFirst.Test.IntegrationTests
{
    [TestClass]
    public class MemoryDbTest
    {
        [TestMethod]
        public void CreateInMemoryDatabaseTest()
        {
            using (DbConnection connection = new SQLiteConnection("data source=:memory:"))
            {
                // This is important! Else the in memory database will not work.
                connection.Open();

                using (var context = new FootballDbContext(connection, false))
                {
                    context.Set<Team>().Add(new Team
                    {
                        Name = "New",
                        Coach = new Coach
                        {
                            City = "New",
                            FirstName = "New",
                            LastName = "New",
                            Street = "New"
                        },
                        Players = new List<Player>
                        {
                            new Player
                            {
                                City = "New",
                                FirstName = "New",
                                LastName = "New",
                                Street = "New",
                                Number = 1
                            },
                            new Player
                            {
                                City = "New",
                                FirstName = "New",
                                LastName = "New",
                                Street = "New",
                                Number = 2
                            }
                        },
                        Stadion = new Stadion
                        {
                            Name = "New",
                            City = "New",
                            Street = "New"
                        }
                    });

                    context.SaveChanges();
                }

                using (var context = new FootballDbContext(connection, false))
                {
                    Assert.AreEqual(1, context.Set<Team>().Count());
                }
            }
        }
    }
}