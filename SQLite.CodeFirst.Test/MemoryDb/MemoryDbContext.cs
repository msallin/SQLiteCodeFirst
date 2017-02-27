using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite.CodeFirst.Test.MemoryDb
{
    [DbConfigurationType(typeof(MemoryDbContextConfiguration))]
    public class MemoryDbContext : DbContext
    {
        public MemoryDbContext() : base("Data Source=:memory:")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity1>()
                .Property(e => e.Id)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Entity1>()
                .HasKey(e => e.Id);

            Database.SetInitializer(new MemoryDbContextInitializer(modelBuilder));
        }

        public virtual DbSet<Entity1> Entity1 { get; set; }
    }
}
