using System;
using System.Data.Entity.Core.Metadata.Edm;
using SQLite.CodeFirst.Builder.NameCreators;
using SQLite.CodeFirst.Utility;

namespace SQLite.CodeFirst.Extensions
{
    internal static class EntityTypeExtension
    {
        public static string GetTableName(this EntityType entityType)
        {
            MetadataProperty metadataProperty;
            if (!entityType.MetadataProperties.TryGetValue("TableName", false, out metadataProperty))
            {
                return entityType.Name;
            }

            if (metadataProperty.Value.GetType().Name != "DatabaseName")
            {
                return entityType.Name;
            }

            object metadataPropertyValue = metadataProperty.Value;
            Type metadataPropertyValueType = metadataProperty.Value.GetType();

            // The type DatabaseName is internal. So we need reflection...
            var name = (string)metadataPropertyValueType.GetProperty("Name").GetValue(metadataPropertyValue);

            return TableNameCreator.CreateTableName(name);
        }
    }
}
