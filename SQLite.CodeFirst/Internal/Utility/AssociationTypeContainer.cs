using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace SQLite.CodeFirst.Utility
{
    internal class AssociationTypeContainer
    {
        private readonly IEnumerable<SqliteAssociationType> sqliteAssociationTypes;

        public AssociationTypeContainer(IEnumerable<AssociationType> associationTypes, EntityContainer container)
        {
            sqliteAssociationTypes = associationTypes.Select(associationType => new SqliteAssociationType(associationType, container));
        }

        public IEnumerable<SqliteAssociationType> GetAssociationTypes(string entitySetName)
        {
            return sqliteAssociationTypes.Where(associationType => associationType.ToRoleEntitySetName == entitySetName);
        }
    }
}
