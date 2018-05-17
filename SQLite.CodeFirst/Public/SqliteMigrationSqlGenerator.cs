using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SQLite.CodeFirst
{
    /// <sumary>
    /// Converts provider-agnostic migration operations into database provider specific SQL commands for SQLite.
    /// </sumary>
    /// <remarks>
    /// In the constructor of your migrations Configuration class, add:
    /// SetSqlGenerator( "System.Data.SQLite", new SQLite.CodeFirst.SqliteMigrationSqlGenerator() );
    /// </remarks>
    public sealed class SqliteMigrationSqlGenerator : MigrationSqlGenerator
    {

        #region Overrides from MigrationSqlGenerator

        /// <summary>
        /// Converts a set of migration operations into database provider specific SQL.
        /// </summary>
        /// <param name="migrationOperations">The operations to be converted.</param>
        /// <param name="providerManifestToken">Token representing the version of the database being targeted.</param>
        /// <returns>A list of SQL statements to be executed to perform the migration operations.</returns>
        public override IEnumerable<MigrationStatement> Generate(
            IEnumerable<MigrationOperation> migrationOperations, string providerManifestToken)
        {
            var impl = new Impl(providerManifestToken);
            impl.Generate(migrationOperations);

            //var text = new StringBuilder();
            //foreach (var migrationStatement in impl.MigrationStatements)
            //{
            //    text.AppendLine(migrationStatement.Sql);
            //}

            //Debug.WriteLine(text);

            return impl.MigrationStatements;
        }
        
        #endregion

        sealed class Impl
        {
            #region Constantes

            const string _providerInvariantName = "System.Data.SQLite";
            const string _defaultDateTimeFormat = "yyyy-MM-dd hh:mm:ss";
            const int _defaultStringMaxLength = 255;
            const int _defaultNumericPrecision = 10;
            const byte _defaultTimePrecision = 7;
            const byte _defaultNumericScale = 0;
            static readonly Regex _rxMatchParameterReference = new Regex("@p[0-9]+", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);

            #endregion

            #region Instancias

            DbProviderServices _providerServices;
            DbProviderManifest _providerManifest;
            List<MigrationStatement> _migrationStatements;

            #endregion

            #region Public Surface

            public Impl(string providerManifestToken)
            {
                _migrationStatements = new List<MigrationStatement>();

                var factory = DbProviderFactories.GetFactory(_providerInvariantName);

                using (var connection = factory.CreateConnection())
                {
                    _providerServices = DbProviderServices.GetProviderServices(connection);
                    _providerManifest = _providerServices.GetProviderManifest(providerManifestToken);
                }
            }

            public IEnumerable<MigrationStatement> MigrationStatements
            {
                get
                {
                    return _migrationStatements;
                }
            }

            public void Generate(IEnumerable<MigrationOperation> migrationOperations)
            {

                foreach (dynamic dynamicOperation in migrationOperations)
                    Generate(dynamicOperation);
            }

            #endregion

            #region Migration Statement Generation

            private void Generate(CreateTableOperation op)
            {
                SetAnnotatedColumns(op.Columns, op.Name);

                using (var tw = CreateIndentedTextWriter())
                {
                    GenerateCreateTableCommand(op, tw);

                    AddSqlStatement(tw);
                }
            }

            private void Generate(AddForeignKeyOperation op)
            {
                if (!_migrationStatements.Any(item => item.Sql.Contains("CREATE TABLE")))
                    return;

                var migrationStatement = _migrationStatements
                    .FirstOrDefault(item => item.Sql
                        .Contains(string.Format(CultureInfo.InvariantCulture, "CREATE TABLE {0} (", FormatReservedWord(RemoveDbo(op.DependentTable)))));

                if (migrationStatement == null)
                    throw new NotSupportedException("SQL command to create the dependent table not found.");

                using (var tw = CreateIndentedTextWriter())
                {
                    tw.Write(migrationStatement.Sql.TrimEnd(')').TrimEnd('\r', '\n'));

                    tw.WriteLine(",");

                    tw.Indent++;

                    var referenceList = op.DependentColumns.Select(dependentColumn => op.PrincipalColumns[op.DependentColumns.IndexOf(dependentColumn)]).ToList();

                    var cmd = string.Format(CultureInfo.InvariantCulture, "FOREIGN KEY ({0}) REFERENCES [{1}] ({2}) {3}",
                        string.Join(", ", op.DependentColumns.Select(FormatReservedWord)),
                        FormatReservedWord(op.PrincipalTable),
                        string.Join(", ", referenceList.Select(FormatReservedWord)),
                        (op.CascadeDelete
                            ? "ON DELETE CASCADE"
                            : "ON DELETE NO ACTION"));

                    tw.WriteLine(RemoveDbo(cmd));

                    tw.Indent--;
                    tw.Write(")");

                    migrationStatement.Sql = tw.InnerWriter.ToString();
                }

            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "op")]
            private static void Generate(DropForeignKeyOperation op)
            {
                // Currently not supported.
                throw new NotSupportedException();
            }

            private void Generate(CreateIndexOperation op)
            {
                using (var tw = CreateIndentedTextWriter())
                {
                    var indexName = op.HasDefaultName
                        ? string.Format(CultureInfo.InvariantCulture, "{0}_{1}", op.Name, RemoveDbo(op.Table))
                        : op.Name;

                    tw.Write("CREATE ");
                    
                    if (op.IsUnique)
                        tw.Write(" UNIQUE ");

                    tw.Write("INDEX ");
                    tw.Write(FormatReservedWord(indexName));
                    tw.Write(" ON ");
                    tw.Write(FormatReservedWord(RemoveDbo(op.Table)));
                    tw.Write("(");

                    for (int i = 0; i < op.Columns.Count; i++)
                    {
                        tw.Write(FormatReservedWord(op.Columns[i]));

                        if (i < op.Columns.Count - 1)
                            tw.WriteLine(",");
                    }

                    tw.Write(")");

                    AddSqlStatement(tw);
                }
            }

            private void Generate(DropIndexOperation op)
            {
                using (var tw = CreateIndentedTextWriter())
                {
                    var indexName = op.HasDefaultName
                        ? string.Format(CultureInfo.InvariantCulture, "{0}_{1}", op.Name, FormatReservedWord(RemoveDbo(op.Table)))
                        : op.Name;

                    tw.Write("DROP INDEX ");
                    tw.Write(indexName);

                    AddSqlStatement(tw);
                }
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "op")]
            private static void Generate(AddPrimaryKeyOperation op)
            {
                throw new NotImplementedException("AddPrimaryKey is non-trivial and has not been implemented. See http://sqlite.org/lang_altertable.html");
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "op")]
            private static void Generate(DropPrimaryKeyOperation op)
            {
                throw new NotImplementedException("DropPrimaryKey is non-trivial and has not been implemented. See http://sqlite.org/lang_altertable.html");
            }

            private void Generate(AddColumnOperation op)
            {
                SetAnnotatedColumn(op.Column, op.Table);

                using (var tw = CreateIndentedTextWriter())
                {
                    tw.Write("ALTER TABLE ");
                    tw.Write(FormatReservedWord(RemoveDbo(op.Table)));
                    tw.Write(" ADD ");

                    var col = op.Column;

                    Generate(col, tw, null);

                    if ((col.IsNullable != null)
                        && !col.IsNullable.Value
                        && (col.DefaultValue == null)
                        && (string.IsNullOrWhiteSpace(col.DefaultValueSql))
                        && !col.IsIdentity
                        && !col.IsTimestamp
                        && (
                            col.StoreType == null || (
                                !col.StoreType.Equals("rowversion", StringComparison.OrdinalIgnoreCase)
                                && !col.StoreType.Equals("timestamp", StringComparison.OrdinalIgnoreCase))))
                    {
                        tw.Write(" DEFAULT ");

                        if (col.Type == PrimitiveTypeKind.DateTime)
                        {
                            tw.Write(Generate(DateTime.Parse("1900-01-01 00:00:00", CultureInfo.InvariantCulture)));
                        }
                        else
                        {
                            tw.Write(Generate((dynamic)col.ClrDefaultValue));
                        }
                    }

                    AddSqlStatement(tw);
                }
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "op")]
            private static void Generate(DropColumnOperation op)
            {
                throw new NotImplementedException("DropColumn is non-trivial and has not been implemented. See http://sqlite.org/lang_altertable.html");
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "op")]
            private static void Generate(AlterColumnOperation op)
            {
                throw new NotImplementedException("AlterColumn is non-trivial and has not been implemented. See http://sqlite.org/lang_altertable.html");
            }

            private void Generate(DropTableOperation op)
            {
                using (var tw = CreateIndentedTextWriter())
                {
                    tw.Write("DROP TABLE ");
                    tw.Write(FormatReservedWord(RemoveDbo(op.Name)));

                    AddSqlStatement(tw);
                }
            }

            private void Generate(SqlOperation opeSQL)
            {
                var sql = RemoveDbo(opeSQL.Sql);

                AddSqlStatement(sql, opeSQL.SuppressTransaction);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "op")]
            private static void Generate(RenameColumnOperation op)
            {
                throw new NotImplementedException("RenameColumn is non-trivial and has not been implemented. See http://sqlite.org/lang_altertable.html");
            }

            private void Generate(RenameTableOperation op)
            {
                using (var tw = CreateIndentedTextWriter())
                {
                    tw.Write("ALTER TABLE ");
                    tw.Write(FormatReservedWord(RemoveDbo(op.Name)));
                    tw.Write(" RENAME TO ");
                    tw.Write(FormatReservedWord(RemoveDbo(op.NewName)));

                    AddSqlStatement(tw);
                }
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "opeMoveTable")]
            private static void Generate(MoveTableOperation opeMoveTable)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Generate column definition. Returns <c>true</c> if the column was the primary key.
            /// </summary>
            private bool Generate(ColumnModel column, IndentedTextWriter tw, PrimaryKeyOperation primaryKeyOp)
            {
                bool isIdPk = false;

                tw.Write(FormatReservedWord(column.Name));
                tw.Write(" ");
                bool isPrimaryKey = false;

                if (primaryKeyOp != null)
                    isPrimaryKey = primaryKeyOp.Columns.Contains(column.Name);

                if (isPrimaryKey)
                {
                    if ((column.Type == PrimitiveTypeKind.Int16) ||
                        (column.Type == PrimitiveTypeKind.Int32))
                        tw.Write("INTEGER");
                    else
                        tw.Write(BuildColumnType(column));

                    if (column.IsIdentity || column.Annotations.ContainsKey("Autoincrement"))
                    {
                        tw.Write(" PRIMARY KEY");
                        isIdPk = true;

                        if (column.Annotations.ContainsKey("Autoincrement"))
                            tw.Write(" AUTOINCREMENT");
                    }
                }
                else
                {
                    tw.Write(BuildColumnType(column));

                    if ((column.IsNullable != null)
                        && !column.IsNullable.Value)
                    {
                        tw.Write(" NOT NULL");
                    }

                    if (column.DefaultValue != null)
                    {
                        tw.Write(" DEFAULT ");
                        tw.Write(Generate((dynamic)column.DefaultValue));
                    }
                    else if (!string.IsNullOrWhiteSpace(column.DefaultValueSql))
                    {
                        tw.Write(" DEFAULT ");
                        tw.Write(column.DefaultValueSql);
                    }

                    AnnotationValues uniqueAnnotation;
                    if (column.Annotations.TryGetValue("Unique", out uniqueAnnotation))
                    {
                        tw.Write(" UNIQUE");
                        var action = GetUniqueConflictAction(uniqueAnnotation);
                        if (action != OnConflictAction.None)
                            tw.Write(" ON CONFLICT " + action.ToString().ToUpperInvariant());
                    }
                }

                return isIdPk;
            }

            private void Generate(HistoryOperation operation)
            {
                foreach (var cmdTree in operation.CommandTrees)
                {
                    // Note: For comparison, within the SqlServer-specific implementation, there's a method equivalent to
                    // CreateCommandDefinition that takes a boolean flag "generateParameters". This controls whether paremeters
                    // are generated as references (ie, "@p0" etc) or literals (ie, "'SomeLiteralString'" etc). There's no
                    // equivalent exposed from System.Data.SQLite. So instead we replace parameter references with the literals
                    // within the generated CommandText. The former approach would, of course, be preferable if it were
                    // available.

                    var cmdDef = _providerServices.CreateCommandDefinition(_providerManifest, cmdTree);
                    var cmd = cmdDef.CreateCommand();
                    var cmdText = cmd.CommandText;

                    if (cmd.Parameters.Count > 0)
                    {
                        cmdText = _rxMatchParameterReference.Replace(
                            cmdText,
                            (Match m) =>
                            {
                                int idx = int.Parse(m.Value.Substring(2), CultureInfo.InvariantCulture);
                                dynamic dynValue = cmd.Parameters[idx].Value;
                                string literal = Generate(dynValue);
                                return literal;
                            });
                    }

                    AddSqlStatement(cmdText);
                }
            }

            /// <summary>
            /// Generate byte array literal.
            /// </summary>
            private static string Generate(byte[] v)
            {
                var sb = new StringBuilder((v.Length * 2) + 3);

                sb.Append("x'");

                foreach (var b in v)
                    sb.Append(b.ToString("X2", CultureInfo.InvariantCulture));

                sb.Append("'");

                return sb.ToString();
            }

            /// <summary>
            /// Generate boolean literal.
            /// </summary>
            private static string Generate(bool v)
            {
                return v ? "1" : "0";
            }

            /// <summary>
            /// Generate DateTime literal.
            /// </summary>
            private static string Generate(DateTime v)
            {
                return "'" + v.ToString(_defaultDateTimeFormat, CultureInfo.InvariantCulture) + "'";
            }

            /// <summary>
            /// Generate DateTimeOffSet literal.
            /// </summary>
            private static string Generate(DateTimeOffset v)
            {
                return "'" + v.ToString(_defaultDateTimeFormat, CultureInfo.InvariantCulture) + "'";
            }

            /// <summary>
            /// Generate Guid literal.
            /// </summary>
            private static string Generate(Guid v)
            {
                return "'" + v + "'";
            }

            /// <summary>
            /// Generate string literal.
            /// </summary>
            private static string Generate(string v)
            {
                return "'" + v.Replace("'", "''") + "'";
            }

            /// <summary>
            /// Generate TimeSpan literal.
            /// </summary>
            private static string Generate(TimeSpan v)
            {
                return "'" + v + "'";
            }

            /// <summary>
            /// Generate literal for other object.
            /// </summary>
            private static string Generate(object v)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}", v);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "tableName")]
            private static void SetAnnotatedColumn(ColumnModel column, string tableName)
            {
                AnnotationValues values;
                if (column.Annotations.TryGetValue("SqlDefaultValue", out values))
                {
                    if (values.NewValue == null)
                    {
                        column.DefaultValueSql = null;
                        using (var writer = CreateIndentedTextWriter())
                        {
                            // Drop Constraint
                            //writer.WriteLine(GetSqlDropConstraintQuery(tableName, column.Name));
                            //Statement(writer);
                        }
                    }
                    else
                    {
                        column.DefaultValueSql = (string)values.NewValue;
                    }
                }
            }

            private static void SetAnnotatedColumns(IEnumerable<ColumnModel> columns, string tableName)
            {
                foreach (var column in columns)
                {
                    SetAnnotatedColumn(column, tableName);
                }
            }

//            private string GetSqlDropConstraintQuery(string tableName, string columnName)
//            {
//                var tableNameSplittedByDot = tableName.Split('.');
//                var tableSchema = tableNameSplittedByDot[0];
//                var tablePureName = tableNameSplittedByDot[1];

//                var str = @"DECLARE @var{dropConstraintCount} nvarchar(128)
//                            SELECT @var{dropConstraintCount} = name
//                            FROM sys.default_constraints
//                            WHERE parent_object_id = object_id(N'{tableSchema}.[{tablePureName}]')
//                            AND col_name(parent_object_id, parent_column_id) = '{columnName}';
//                            IF @var{dropConstraintCount} IS NOT NULL
//                                EXECUTE('ALTER TABLE {tableSchema}.[{tablePureName}] DROP CONSTRAINT [' + @var{dropConstraintCount} + ']')";

//                dropConstraintCount = dropConstraintCount + 1;
//                return str;
//            }


            #endregion

            #region Support methods

            /// <summary>
            /// Builds a column type
            /// </summary>
            /// <returns> SQL representing the data type. </returns>
            private string BuildColumnType(ColumnModel column)
            {
                return column.IsTimestamp ? "rowversion" : BuildPropertyType(column);
            }

            /// <summary>
            /// Builds a SQL property type fragment from the specified <see cref="ColumnModel"/>.
            /// </summary>
            /// <param name="column"></param>
            /// <returns></returns>
            private string BuildPropertyType(ColumnModel column)
            {
                var originalStoreType = column.StoreType;

                if (string.IsNullOrWhiteSpace(originalStoreType))
                {
                    var typeUsage = _providerManifest.GetStoreType(column.TypeUsage).EdmType;
                    originalStoreType = typeUsage.Name;
                }

                var storeType = originalStoreType;

                const string maxSuffix = "(max)";

                if (storeType.EndsWith(maxSuffix, StringComparison.Ordinal))
                    storeType = storeType.Substring(0, storeType.Length - maxSuffix.Length) + maxSuffix;

                switch (originalStoreType.ToUpperInvariant())
                {
                    case "DECIMAL":
                    case "NUMERIC":
                        storeType += "(" + (column.Precision ?? _defaultNumericPrecision)
                                         + ", " + (column.Scale ?? _defaultNumericScale) + ")";
                        break;
                    case "DATETIME":
                    case "TIME":
                        storeType += "(" + (column.Precision ?? _defaultTimePrecision) + ")";
                        break;
                    case "BLOB":
                    case "VARCHAR2":
                    case "VARCHAR":
                    case "CHAR":
                    case "NVARCHAR":
                    case "NVARCHAR2":
                        storeType += "(" + (column.MaxLength ?? _defaultStringMaxLength) + ")";
                        break;
                }

                return storeType;
            }

            /// <summary>
            /// Adds a new Statement to be executed against the database.
            /// </summary>
            /// <param name="sql"></param>
            /// <param name="suppressTransaction"></param>
            private void AddSqlStatement(string sql, bool suppressTransaction = false)
            {
                _migrationStatements.Add(new MigrationStatement
                {
                    Sql = sql,
                    SuppressTransaction = suppressTransaction
                });
            }

            /// <summary>
            /// Adds a new Statement to be executed against the database.
            /// </summary>
            /// <param name="tw"> The writer containing the SQL to be executed. </param>
            private void AddSqlStatement(IndentedTextWriter tw)
            {
                AddSqlStatement(tw.InnerWriter.ToString());
            }

            /// <summary>
            /// Gera um objeto <see cref="IndentedTextWriter" /> Utilizado para gerar os comandos SQL.
            /// </summary>
            /// <returns> An empty text writer to use for SQL generation. </returns>
            private static IndentedTextWriter CreateIndentedTextWriter()
            {
                var writer = new StringWriter(CultureInfo.InvariantCulture);
                try
                {
                    return new IndentedTextWriter(writer);
                }
                catch
                {
                    writer.Dispose();
                    throw;
                }
            }

            /// <summary>
            /// Remove occurences of "dbo." from the supplied string.
            /// </summary>
            private static string RemoveDbo(string str)
            {
                return str.Replace("dbo.", string.Empty);
            }

            /// <summary>
            /// Surround with double-quotes Sqlite reserved words.
            /// </summary>
            private static string FormatReservedWord(string word)
            {
                switch (word.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "ABORT":
                    case "ACTION":
                    case "ADD":
                    case "AFTER":
                    case "ALL":
                    case "ALTER":
                    case "ANALYZE":
                    case "AND":
                    case "AS":
                    case "ASC":
                    case "ATTACH":
                    case "AUTOINCREMENT":
                    case "BEFORE":
                    case "BEGIN":
                    case "BETWEEN":
                    case "BY":
                    case "CASCADE":
                    case "CASE":
                    case "CAST":
                    case "CHECK":
                    case "COLLATE":
                    case "COLUMN":
                    case "COMMIT":
                    case "CONFLICT":
                    case "CONSTRAINT":
                    case "CREATE":
                    case "CROSS":
                    case "CURRENT_DATE":
                    case "CURRENT_TIME":
                    case "CURRENT_TIMESTAMP":
                    case "DATABASE":
                    case "DEFAULT":
                    case "DEFERRABLE":
                    case "DEFERRED":
                    case "DELETE":
                    case "DESC":
                    case "DETACH":
                    case "DISTINCT":
                    case "DROP":
                    case "EACH":
                    case "ELSE":
                    case "END":
                    case "ESCAPE":
                    case "EXCEPT":
                    case "EXCLUSIVE":
                    case "EXISTS":
                    case "EXPLAIN":
                    case "FAIL":
                    case "FOR":
                    case "FOREIGN":
                    case "FROM":
                    case "FULL":
                    case "GLOB":
                    case "GROUP":
                    case "HAVING":
                    case "IF":
                    case "IGNORE":
                    case "IMMEDIATE":
                    case "IN":
                    case "INDEX":
                    case "INDEXED":
                    case "INITIALLY":
                    case "INNER":
                    case "INSERT":
                    case "INSTEAD":
                    case "INTERSECT":
                    case "INTO":
                    case "IS":
                    case "ISNULL":
                    case "JOIN":
                    case "KEY":
                    case "LEFT":
                    case "LIKE":
                    case "LIMIT":
                    case "MATCH":
                    case "NATURAL":
                    case "NO":
                    case "NOT":
                    case "NOTNULL":
                    case "NULL":
                    case "OF":
                    case "OFFSET":
                    case "ON":
                    case "OR":
                    case "ORDER":
                    case "OUTER":
                    case "PLAN":
                    case "PRAGMA":
                    case "PRIMARY":
                    case "QUERY":
                    case "RAISE":
                    case "RECURSIVE":
                    case "REFERENCES":
                    case "REGEXP":
                    case "REINDEX":
                    case "RELEASE":
                    case "RENAME":
                    case "REPLACE":
                    case "RESTRICT":
                    case "RIGHT":
                    case "ROLLBACK":
                    case "ROW":
                    case "SAVEPOINT":
                    case "SELECT":
                    case "SET":
                    case "TABLE":
                    case "TEMP":
                    case "TEMPORARY":
                    case "THEN":
                    case "TO":
                    case "TRANSACTION":
                    case "TRIGGER":
                    case "UNION":
                    case "UNIQUE":
                    case "UPDATE":
                    case "USING":
                    case "VACUUM":
                    case "VALUES":
                    case "VIEW":
                    case "VIRTUAL":
                    case "WHEN":
                    case "WHERE":
                    case "WITH":
                    case "WITHOUT":
                        return '"' + word + '"';

                    default:
                        return word;
                }
            }

            /// <summary>
            /// Generate CREATE TABLE command
            /// </summary>
            private void GenerateCreateTableCommand(CreateTableOperation op, IndentedTextWriter tw)
            {
                tw.WriteLine("CREATE TABLE " + FormatReservedWord(RemoveDbo(op.Name)) + " (");
                tw.Indent++;

                bool hasGeneratedIdPk = false;

                for (int i = 0; i < op.Columns.Count; i++)
                {
                    ColumnModel lcmDadosColuna = op.Columns.ToList()[i];
                    hasGeneratedIdPk |= Generate(lcmDadosColuna, tw, op.PrimaryKey);
                    
                    if (i < op.Columns.Count - 1)
                        tw.WriteLine(",");
                }

                if ((op.PrimaryKey != null) && !hasGeneratedIdPk)
                {
                    tw.WriteLine(",");
                    tw.Write("CONSTRAINT ");
                    tw.Write(RemoveDbo(op.PrimaryKey.Name));
                    tw.Write(" PRIMARY KEY ");
                    tw.Write("(");

                    for (int li = 0; li < op.PrimaryKey.Columns.Count; li++)
                    {
                        var lstrNomeColuna = op.PrimaryKey.Columns[li];

                        tw.Write(lstrNomeColuna);

                        if (li < op.PrimaryKey.Columns.Count - 1)
                            tw.WriteLine(",");
                    }

                    tw.WriteLine(")");
                }
                else
                {
                    tw.WriteLine();
                }

                tw.Indent--;
                tw.Write(")");
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Enum.TryParse<SQLite.CodeFirst.OnConflictAction>(System.String,SQLite.CodeFirst.OnConflictAction@)")]
            private static OnConflictAction GetUniqueConflictAction(AnnotationValues uniqueAnnotation)
            {
                var uniqueText = Convert.ToString(uniqueAnnotation.NewValue, CultureInfo.InvariantCulture);
                var action = OnConflictAction.None;

                if (uniqueText.StartsWith("OnConflict:", StringComparison.OrdinalIgnoreCase))
                {
                    var actionText = uniqueText.Remove(0, "OnConflict:".Length).Trim();
                    Enum.TryParse(actionText, out action);
                }

                return action;
            }

            #endregion
        }
    }
}