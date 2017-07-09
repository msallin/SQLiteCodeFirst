using System;
using System.Data.Entity.Core.Metadata.Edm;
using SQLite.CodeFirst.Builder.NameCreators;

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
            // GetValue() overload with one value was introduces in .net 4.5 so use the overload with two parameters. 
            var name = (string)metadataPropertyValueType.GetProperty("Name").GetValue(metadataPropertyValue, null);

            return NameCreator.EscapeName(name);
        }
    }
}
