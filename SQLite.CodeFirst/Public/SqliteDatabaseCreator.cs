using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;

namespace SQLite.CodeFirst
{
    /// <summary>
    /// Creates a SQLite-Database based on a Entity Framework <see cref="Database"/> and <see cref="DbModel"/>.
    /// This creator can be used standalone or within an initializer.
    /// <remark>
    /// The generated DDL-SQL will be executed together as one statement.
    /// If there is a open transaction on the Database, the statement will be executed within this transaction.
    /// Otherwise it will be executed within a own transaction. In anyway the atomicity is guaranteed.
    /// </remark>
    /// </summary>
    public class SqliteDatabaseCreator : IDatabaseCreator
    {
        /// <summary>
        /// Creates the SQLite-Database.
        /// </summary>
        public void Create(Database db, DbModel model)
        {
            if (db == null) throw new ArgumentNullException("db");
            if (model == null) throw new ArgumentNullException("model");

            var sqliteSqlGenerator = new SqliteSqlGenerator();
            string sql = sqliteSqlGenerator.Generate(model.StoreModel);
            Debug.Write(sql);
            db.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, sql);
        }
    }
}
