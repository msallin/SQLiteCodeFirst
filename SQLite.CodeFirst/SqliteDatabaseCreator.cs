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
        private readonly Database db;
        private readonly DbModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteDatabaseCreator"/> class.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="model">The model.</param>
        public SqliteDatabaseCreator(Database db, DbModel model)
        {
            this.db = db;
            this.model = model;
        }

        /// <summary>
        /// Creates the SQLite-Database.
        /// </summary>
        public void Create()
        {
            var sqliteSqlGenerator = new SqliteSqlGenerator(model.StoreModel);
            string sql = sqliteSqlGenerator.Generate();
            db.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, sql);
        }
    }
}
