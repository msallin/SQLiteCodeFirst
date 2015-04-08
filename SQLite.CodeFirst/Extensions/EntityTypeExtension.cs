using System.Data.Entity.Core.Metadata.Edm;

namespace SQLite.CodeFirst.Extensions
{
    internal static class EntityTypeExtension
    {
        public static string GetTableName(this EntityType entityType)
        {
            MetadataProperty metadataProperty;
            if (entityType.MetadataProperties.TryGetValue("TableName", false, out metadataProperty))
            {
                return metadataProperty.Value.ToString();
            }

            return entityType.Name;
        }
    }
}
