using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SQLite.CodeFirst.Test.IntegrationTests
{
    /// <summary>
    /// Verifies that the <see cref="CascadeOnUpdateAttribute"/> placed on a foreign key property
    /// results in 'ON UPDATE CASCADE' being emitted for the corresponding foreign key constraint.
    /// This exercises the full pipeline: the attribute is registered as a column annotation, survives
    /// the EF model build and is read back from the store model when the SQL is generated.
    /// </summary>
    [TestClass]
    public class CascadeOnUpdateTest
    {
        private static string generatedSql;

        [TestMethod]
        public void CascadeOnUpdateAttributeEmitsOnUpdateCascade()
        {
            using (DbConnection connection = new SQLiteConnection("FullUri=file::memory:"))
            {
                // This is important! Else the in memory database will not work.
                connection.Open();

                using (var context = new CascadeDbContext(connection))
                {
                    // Touching the set forces the initializer (and therefore the SQL generation) to run.
                    context.Set<Order>().FirstOrDefault();
                }
            }

            // Decorated FK with cascade-on-delete: both keywords, delete before update.
            StringAssert.Contains(generatedSql, "FOREIGN KEY ([CustomerId]) REFERENCES \"Customers\"([Id]) ON DELETE CASCADE ON UPDATE CASCADE");

            // Decorated FK without cascade-on-delete: only the update keyword.
            StringAssert.Contains(generatedSql, "FOREIGN KEY ([WarehouseId]) REFERENCES \"Warehouses\"([Id]) ON UPDATE CASCADE");

            // Undecorated FK: no cascade keyword at all.
            Assert.IsFalse(generatedSql.Contains("[RegionId]) REFERENCES \"Regions\"([Id]) ON"),
                "The foreign key without the attribute must not emit any cascade clause.");
        }

        private class Customer
        {
            public int Id { get; set; }
        }

        private class Warehouse
        {
            public int Id { get; set; }
        }

        private class Region
        {
            public int Id { get; set; }
        }

        private class Order
        {
            public int Id { get; set; }

            [CascadeOnUpdate]
            public int CustomerId { get; set; }
            public Customer Customer { get; set; }

            [CascadeOnUpdate]
            public int? WarehouseId { get; set; }
            public Warehouse Warehouse { get; set; }

            public int? RegionId { get; set; }
            public Region Region { get; set; }
        }

        private class CascadeDbContext : DbContext
        {
            public CascadeDbContext(DbConnection connection)
                : base(connection, false)
            {
            }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Order>()
                    .HasRequired(o => o.Customer)
                    .WithMany()
                    .HasForeignKey(o => o.CustomerId)
                    .WillCascadeOnDelete(true);

                modelBuilder.Entity<Order>()
                    .HasOptional(o => o.Warehouse)
                    .WithMany()
                    .HasForeignKey(o => o.WarehouseId)
                    .WillCascadeOnDelete(false);

                modelBuilder.Entity<Order>()
                    .HasOptional(o => o.Region)
                    .WithMany()
                    .HasForeignKey(o => o.RegionId)
                    .WillCascadeOnDelete(false);

                Database.SetInitializer(new CaptureInitializer(modelBuilder));
            }
        }

        private class CaptureInitializer : SqliteInitializerBase<CascadeDbContext>
        {
            public CaptureInitializer(DbModelBuilder modelBuilder)
                : base(modelBuilder)
            {
            }

            public override void InitializeDatabase(CascadeDbContext context)
            {
                DbModel model = ModelBuilder.Build(context.Database.Connection);
                generatedSql = new SqliteSqlGenerator().Generate(model.StoreModel);
                base.InitializeDatabase(context);
            }
        }
    }
}
