using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;
using System.Diagnostics;
using System.Text;
using SQLite.CodeFirst.Builder;

namespace SQLite.CodeFirst
{
    /// <sumary>
    ///     Converts provider-agnostic migration operations into database provider specific SQL commands for SQLite.
    /// </sumary>
    /// <remarks>
    ///     In the constructor of your migrations Configuration class, add:
    ///     SetSqlGenerator( "System.Data.SQLite", new SQLite.CodeFirst.SqliteMigrationSqlGenerator() );
    /// </remarks>
    public sealed class SqliteMigrationSqlGenerator : MigrationSqlGenerator
    {
        /// <summary>
        ///     Converts a set of migration operations into database provider specific SQL.
        /// </summary>
        /// <param name="migrationOperations">The operations to be converted.</param>
        /// <param name="providerManifestToken">Token representing the version of the database being targeted.</param>
        /// <returns>A list of SQL statements to be executed to perform the migration operations.</returns>
        public override IEnumerable<MigrationStatement> Generate(IEnumerable<MigrationOperation> migrationOperations, string providerManifestToken)
        {
            var migrationBuilder = new MigrationBuilder(providerManifestToken);
            migrationBuilder.Generate(migrationOperations);
#if DEBUG
            var text = new StringBuilder();
            foreach (var migrationStatement in migrationBuilder.MigrationStatements)
            {
                text.AppendLine(migrationStatement.Sql);
            }

            Debug.WriteLine(text);
#endif
            return migrationBuilder.MigrationStatements;
        }
    }
}