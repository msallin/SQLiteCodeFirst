using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace SQLite.CodeFirst
{
    /// <summary>
    /// Creates a SQLite-Database based on a Entity Framework <see cref="Database"/> and <see cref="DbModel"/>.
    /// This creator can be used standalone or within an initializer.
    /// </summary>
    public class SqliteDatabaseCreator : IDatabaseCreator
    {
        /// <summary>
        /// Creates the SQLite-Database.
        /// </summary>
        public void Create(Database db, DbModel model)
        {
            var sqliteSqlGenerator = new SqliteSqlGenerator();
            string sql = sqliteSqlGenerator.Generate(model.StoreModel);
            db.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, sql);
        }
    }
}
