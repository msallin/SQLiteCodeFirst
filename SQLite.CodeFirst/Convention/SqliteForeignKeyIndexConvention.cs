using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace SQLite.CodeFirst.Convention
{
    public class SqliteForeignKeyIndexConvention : IStoreModelConvention<AssociationType>
    {
        private const string IndexAnnotationName = "http://schemas.microsoft.com/ado/2013/11/edm/customannotation:Index";

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

            for (int i = 0; i < item.Constraint.ToProperties.Count; i++)
            {
                EdmProperty edmProperty = item.Constraint.ToProperties[i];
                var annotation = GetAnnotation(edmProperty.MetadataProperties, IndexAnnotationName);
                if (annotation != null)
                {
                    // The original attribute is removed. The noneForeignKeyIndicies will be remained and readded without any modification
                    // and the foreignKeyIncidies will be readded with the correct name.
                    edmProperty.RemoveAnnotation(IndexAnnotationName);

                    var noneForeignKeyIndicies = annotation.Indexes.Where(index => index.Name != "IX_" + edmProperty.Name);
                    IndexAnnotation newIndexAnnotation = new IndexAnnotation(noneForeignKeyIndicies);

                    var foreignKeyIndicies = annotation.Indexes.Where(index => index.Name == "IX_" + edmProperty.Name);
                    foreach (var foreignKeyIndex in foreignKeyIndicies)
                    {
                        var indexAttribute = new IndexAttribute(string.Format("IX_{0}_{1}", item.Constraint.ToRole.Name, edmProperty.Name));
                        IndexAnnotation foreignKeyIndexAnnotation = new IndexAnnotation(indexAttribute);
                        newIndexAnnotation = (IndexAnnotation)newIndexAnnotation.MergeWith(foreignKeyIndexAnnotation);
                    }

                    edmProperty.AddAnnotation(IndexAnnotationName, newIndexAnnotation);
                }
            }
        }


        private static IndexAnnotation GetAnnotation(IEnumerable<MetadataProperty> metadataProperties, string name)
        {
            foreach (MetadataProperty metadataProperty in metadataProperties)
            {
                if (metadataProperty.Name.Equals(name, StringComparison.Ordinal))
                {
                    return (IndexAnnotation)metadataProperty.Value;
                }
            }
            return null;
        }
    }
}