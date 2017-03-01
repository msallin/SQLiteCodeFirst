using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace SQLite.CodeFirst
{
    public class SqliteMigrateDatabaseToLatestVersion<TContext, TMigrationsConfiguration> : MigrateDatabaseToLatestVersion<TContext, TMigrationsConfiguration>
        where TContext : DbContext
        where TMigrationsConfiguration : DbMigrationsConfiguration<TContext>, new()
    {
        /// <summary>
        /// Initializes a new instance of the SqliteMigrateDatabaseToLatestVersion class that will use
        ///             the connection information from a context constructed using the default constructor
        ///             or registered factory if applicable
        /// 
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created. </param>
        public SqliteMigrateDatabaseToLatestVersion(DbModelBuilder modelBuilder)
            : base()
        {
            Initialize(modelBuilder);
        }

        /// <summary>
        /// Initializes a new instance of the SqliteMigrateDatabaseToLatestVersion class specifying whether to
        ///             use the connection information from the context that triggered initialization to perform the migration.
        /// 
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created. </param>
        /// <param name="useSuppliedContext">If set to <c>true</c> the initializer is run using the connection information from the context that
        ///             triggered initialization. Otherwise, the connection information will be taken from a context constructed
        ///             using the default constructor or registered factory if applicable.
        ///             </param>
        public SqliteMigrateDatabaseToLatestVersion(DbModelBuilder modelBuilder, bool useSuppliedContext)
            : base(useSuppliedContext)
        {
            Initialize(modelBuilder);
        }

        /// <summary>
        /// Initializes a new instance of the SqliteMigrateDatabaseToLatestVersion class specifying whether to
        ///             use the connection information from the context that triggered initialization to perform the migration.
        ///             Also allows specifying migrations configuration to use during initialization.
        /// 
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created. </param>
        /// <param name="useSuppliedContext">If set to <c>true</c> the initializer is run using the connection information from the context that
        ///             triggered initialization. Otherwise, the connection information will be taken from a context constructed
        ///             using the default constructor or registered factory if applicable.
        ///             </param><param name="configuration">Migrations configuration to use during initialization. </param>
        public SqliteMigrateDatabaseToLatestVersion(DbModelBuilder modelBuilder, bool useSuppliedContext, TMigrationsConfiguration configuration)
            : base(useSuppliedContext, configuration)
        {
            Initialize(modelBuilder);
        }

        /// <summary>
        /// Initializes a new instance of the SqliteMigrateDatabaseToLatestVersion class that will
        ///             use a specific connection string from the configuration file to connect to
        ///             the database to perform the migration.
        /// 
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created. </param>
        /// <param name="connectionStringName">The name of the connection string to use for migration. </param>
        public SqliteMigrateDatabaseToLatestVersion(DbModelBuilder modelBuilder, string connectionStringName)
            : base(connectionStringName)
        {
            Initialize(modelBuilder);
        }


        private static void Initialize(DbModelBuilder modelBuilder)
        {
            // There is some functionality which is supported by SQLite which can not be covered
            // by using the data annotation attributes from the .net framework.
            // So there are some custom attributes.
            modelBuilder.Conventions.Add(new IConvention[]
            {
                new AttributeToColumnAnnotationConvention<AutoincrementAttribute, bool>("Autoincrement", (p, attributes) => true),
                new AttributeToColumnAnnotationConvention<CollateAttribute, string>("Collate", (p, attributes) => FormatCollationValue(attributes.Single())),
                new AttributeToColumnAnnotationConvention<UniqueAttribute, string>("Unique", (p, attributes) => FormatUniqueValue(attributes.Single())),
                new AttributeToColumnAnnotationConvention<SqlDefaultValueAttribute, string>("SqlDefaultValue", (p, attributes) => attributes.Single().DefaultValue),
            });
        }

        private static string FormatUniqueValue(UniqueAttribute uniqueAttribute)
        {
            return "OnConflict: " + uniqueAttribute.OnConflict;
        }

        private static string FormatCollationValue(CollateAttribute collationAttribute)
        {
            var collationText = collationAttribute.Collation.ToString();
            if (!string.IsNullOrEmpty(collationAttribute.Function))
                collationText += ": " + collationAttribute.Function;
            return collationText;
        }
    }
}
