using System;
using System.Data.Entity.Core.Metadata.Edm;

namespace SQLite.CodeFirst.Extensions
{
    internal static class EdmPropertyExtensions
    {
        public static TAttribute GetCustomAnnotation<TAttribute>(this EdmProperty property)
            where TAttribute : Attribute
        {
            MetadataProperty item;
            bool found = property.MetadataProperties.TryGetValue("http://schemas.microsoft.com/ado/2013/11/edm/customannotation:" + typeof(TAttribute).Name, true, out item);
            if (found)
            {
                return (TAttribute) item.Value;
            }

            return null;
        }
    }
}
