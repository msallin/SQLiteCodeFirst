using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite.CodeFirst.Test.MemoryDb
{
    public class MemoryDbContextInitializer : SqliteDropCreateDatabaseAlways<MemoryDbContext>
    {
        public MemoryDbContextInitializer(DbModelBuilder modelBuilder)
            : base(modelBuilder)
        {
        }

        protected override void Seed(MemoryDbContext context)
        {
            base.Seed(context);
        }
    }
}
