using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace SQLite.CodeFirst
{
    public interface IDatabaseCreator
    {
        void Create(Database db, DbModel model);
    }
}