using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using SQLite.CodeFirst.Extensions;
using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst.Builder
{
    internal class CreateIndexStatementBuilder : IStatementBuilder<CreateIndexStatementCollection>
    {
        private readonly EntityType entityType;

        public CreateIndexStatementBuilder(EntityType entityType)
        {
            this.entityType = entityType;
        }

        public CreateIndexStatementCollection BuildStatement()
        {
            IDictionary<string, CreateIndexStatement> createIndexStatments = new Dictionary<string, CreateIndexStatement>();

            foreach (var edmProperty in entityType.Properties)
            {
                var indexAnnotations = edmProperty.MetadataProperties
                    .Select(x => x.Value)
                    .OfType<IndexAnnotation>();

                foreach (var index in indexAnnotations.SelectMany(ia => ia.Indexes))
                {
                    CreateIndexStatement createIndexStatement;
                    string indexName = GetIndexName(entityType, edmProperty, index);
                    if (!createIndexStatments.TryGetValue(indexName, out createIndexStatement))
                    {
                        createIndexStatement = new CreateIndexStatement
                        {
                            IsUnique = index.IsUnique,
                            Name = indexName,
                            Table = entityType.GetTableName(),
                            Columns = new Collection<CreateIndexStatement.IndexColumn>()
                        };
                        createIndexStatments.Add(indexName, createIndexStatement);
                    }

                    createIndexStatement.Columns.Add(new CreateIndexStatement.IndexColumn
                    {
                        Name = edmProperty.Name,
                        Order = index.Order
                    });
                }
            }

            return new CreateIndexStatementCollection(createIndexStatments.Values);
        }

        private static string GetIndexName(EntityType entityType, EdmProperty property, IndexAttribute index)
        {
            return index.Name ?? string.Format("IX_{0}_{1}", entityType.GetTableName(), property.Name);
        }
    }
}
