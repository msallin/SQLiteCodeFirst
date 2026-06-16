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
            // Materialize once. GetAssociationTypes is called per entity set, so a deferred query
            // would rebuild every SqliteAssociationType (and its entity-set lookups) on each call.
            sqliteAssociationTypes = associationTypes.Select(associationType => new SqliteAssociationType(associationType, container)).ToList();
        }

        public IEnumerable<SqliteAssociationType> GetAssociationTypes(string entitySetName)
        {
            return sqliteAssociationTypes.Where(associationType => associationType.ToRoleEntitySetName == entitySetName);
        }
    }
}
