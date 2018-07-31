using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.Console;
using SQLite.CodeFirst.Console.Entity;

namespace SQLite.CodeFirst.Test.IntegrationTests
{

    [TestClass]
    public class InMemoryDbCreationTest : InMemoryDbTest<FootballDbContext>
    {
        [TestMethod]
        public void CreateInMemoryDatabaseTest()
        {
            using (var context = GetDbContext())
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

            using (var context = GetDbContext())
            {
                Assert.AreEqual(1, context.Set<Team>().Count());
            }
        }
    }
}
