using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using SQLite.CodeFirst.Builder.NameCreators;

namespace SQLite.CodeFirst.Utility
{
    internal class SqliteAssociationType
    {
        private const string SelfReferencingPostfix = "Self";

        public SqliteAssociationType(AssociationType associationType, EntityContainer container)
        {
            FromRoleEntitySetName = associationType.Constraint.FromRole.Name;
            ToRoleEntitySetName = associationType.Constraint.ToRole.Name;

            string fromTable = container.GetEntitySetByName(FromRoleEntitySetName, true).Table;
            string toTable;

            if (IsSelfReferencing(associationType))
            {
                toTable = fromTable;
                ToRoleEntitySetName = FromRoleEntitySetName;
            }
            else
            {
                toTable = container.GetEntitySetByName(ToRoleEntitySetName, true).Table;
            }

            FromTableName = TableNameCreator.CreateTableName(fromTable);
            ToTableName = TableNameCreator.CreateTableName(toTable);
            ForeignKey = associationType.Constraint.ToProperties.Select(x => x.Name);
            ForeignPrimaryKey = associationType.Constraint.FromProperties.Select(x => x.Name);
            CascadeDelete = associationType.Constraint.FromRole.DeleteBehavior == OperationAction.Cascade;
        }

        private static bool IsSelfReferencing(AssociationType associationType)
        {
            string to = associationType.Constraint.ToRole.Name;
            string from = associationType.Constraint.FromRole.Name;

            if (to.Length <= SelfReferencingPostfix.Length)
            {
                return false;
            }

            return to.Remove(to.Length - SelfReferencingPostfix.Length, SelfReferencingPostfix.Length) == from;
        }

        public string ToRoleEntitySetName { get; set; }
        public string FromRoleEntitySetName { get; set; }
        public IEnumerable<string> ForeignKey { get; }
        public string FromTableName { get; }
        public string ToTableName { get; }
        public IEnumerable<string> ForeignPrimaryKey { get; }
        public bool CascadeDelete { get; }
    }
}