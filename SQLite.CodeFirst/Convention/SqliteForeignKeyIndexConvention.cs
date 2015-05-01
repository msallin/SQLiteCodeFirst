using System;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SQLite.CodeFirst.Convention
{
    public class SqliteForeignKeyIndexConvention : IStoreModelConvention<AssociationType>
    {
        public virtual void Apply(AssociationType item, DbModel model)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            if (item.Constraint == null)
            {
                return;
            }
        }
    }
}