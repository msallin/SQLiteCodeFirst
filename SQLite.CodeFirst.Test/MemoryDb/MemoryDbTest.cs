using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite.CodeFirst.Test.MemoryDb
{
    [TestClass]
    public class MemoryDbTest
    {
        [TestMethod]
        public void MemoryDbContext_Test_Create()
        {
            using (MemoryDbContext ctx = new MemoryDbContext())
            {
                ctx.Database.Connection.Open();

                Entity1 e1 = ctx.Entity1.Create();
                e1.Name = "Name";
                ctx.Entity1.Add(e1);
                ctx.SaveChanges();

                Assert.AreEqual(ctx.Entity1.Count(), 1);
            }
        }
    }
}
