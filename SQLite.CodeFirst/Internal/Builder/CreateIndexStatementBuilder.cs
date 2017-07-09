using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using SQLite.CodeFirst.Builder.NameCreators;
using SQLite.CodeFirst.Extensions;
using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst.Builder
{
    internal class CreateIndexStatementBuilder : IStatementBuilder<CreateIndexStatementCollection>
    {
        private readonly EntitySet entitySet;

        public CreateIndexStatementBuilder(EntitySet entitySet)
        {
            this.entitySet = entitySet;
        }

        public CreateIndexStatementCollection BuildStatement()
        {
            IDictionary<string, CreateIndexStatement> createIndexStatments = new Dictionary<string, CreateIndexStatement>();

            foreach (var edmProperty in entitySet.ElementType.Properties)
            {
                var indexAnnotations = edmProperty.MetadataProperties
                    .Select(x => x.Value)
                    .OfType<IndexAnnotation>();

                string tableName = NameCreator.EscapeName(entitySet.Table);
                foreach (var index in indexAnnotations.SelectMany(ia => ia.Indexes))
                {
                    CreateIndexStatement createIndexStatement;
                    string indexName = GetIndexName(index, edmProperty);
                    if (!createIndexStatments.TryGetValue(indexName, out createIndexStatement))
                    {
                        createIndexStatement = new CreateIndexStatement
                        {
                            IsUnique = index.IsUnique,
                            Name = indexName,
                            Table = tableName,
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

        private string GetIndexName(IndexAttribute index, EdmProperty property)
        {
            return index.Name == null ? IndexNameCreator.CreateName(entitySet.ElementType.GetTableName(), property.Name) : NameCreator.EscapeName(index.Name);
        }
    }
}
