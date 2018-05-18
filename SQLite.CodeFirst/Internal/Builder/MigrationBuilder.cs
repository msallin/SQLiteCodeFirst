using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Format = SQLite.CodeFirst.Utility.MigrationFormatter;

namespace SQLite.CodeFirst.Builder
{
    internal class MigrationBuilder
    {
        #region Constantes

        const string _providerInvariantName = "System.Data.SQLite";
        const string _defaultDateTimeFormat = "yyyy-MM-dd hh:mm:ss";
        static readonly Regex _rxMatchParameterReference = new Regex("@p[0-9]+", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);

        #endregion

        #region Instancias

        DbProviderServices _providerServices;
        DbProviderManifest _providerManifest;
        List<MigrationStatement> _migrationStatements;

        #endregion

        #region Public Surface

        public MigrationBuilder(string providerManifestToken)
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

        internal void Generate(IEnumerable<MigrationOperation> migrationOperations)
        {

            foreach (dynamic dynamicOperation in migrationOperations)
                Generate(dynamicOperation);
        }

        #endregion

        #region Migration Statement Generation

        private void Generate(CreateTableOperation op)
        {
            SetAnnotatedColumns(op.Columns, op.Name);

            using (var tw = Format.CreateIndentedTextWriter())
            {
                GenerateCreateTableCommand(op, tw);

                AddSqlStatement(tw);
            }
        }

        private void Generate(AddForeignKeyOperation op)
        {
            if (!_migrationStatements.Any(item => item.Sql.Contains("CREATE TABLE")))
                return;

            var createScript = string.Format(CultureInfo.InvariantCulture, 
                "CREATE TABLE {0} (", Format.ReservedWord(Format.RemoveDbo(op.DependentTable)));

            var migrationStatement = _migrationStatements
                .FirstOrDefault(item => item.Sql
                    .Contains(createScript));

            if (migrationStatement == null)
                throw new NotSupportedException("SQL command to create the dependent table not found.");

            using (var tw = Format.CreateIndentedTextWriter())
            {
                tw.Write(migrationStatement.Sql.TrimEnd(')').TrimEnd('\r', '\n'));

                tw.WriteLine(",");

                tw.Indent++;

                var referenceList = op.DependentColumns.Select(dependentColumn => op.PrincipalColumns[op.DependentColumns.IndexOf(dependentColumn)]).ToList();

                var cmd = string.Format(CultureInfo.InvariantCulture, "FOREIGN KEY ({0}) REFERENCES [{1}] ({2}) {3}",
                    string.Join(", ", op.DependentColumns.Select(Format.ReservedWord)),
                    Format.ReservedWord(op.PrincipalTable),
                    string.Join(", ", referenceList.Select(Format.ReservedWord)),
                    (op.CascadeDelete
                        ? "ON DELETE CASCADE"
                        : "ON DELETE NO ACTION"));

                tw.WriteLine(Format.RemoveDbo(cmd));

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
            using (var tw = Format.CreateIndentedTextWriter())
            {
                var indexName = op.HasDefaultName
                    ? string.Format(CultureInfo.InvariantCulture, "{0}_{1}", op.Name, Format.RemoveDbo(op.Table))
                    : op.Name;

                tw.Write("CREATE ");

                if (op.IsUnique)
                    tw.Write(" UNIQUE ");

                tw.Write("INDEX ");
                tw.Write(Format.ReservedWord(indexName));
                tw.Write(" ON ");
                tw.Write(Format.ReservedWord(Format.RemoveDbo(op.Table)));
                tw.Write("(");

                for (int i = 0; i < op.Columns.Count; i++)
                {
                    tw.Write(Format.ReservedWord(op.Columns[i]));

                    if (i < op.Columns.Count - 1)
                        tw.WriteLine(",");
                }

                tw.Write(")");

                AddSqlStatement(tw);
            }
        }

        private void Generate(DropIndexOperation op)
        {
            using (var tw = Format.CreateIndentedTextWriter())
            {
                var indexName = op.HasDefaultName
                    ? string.Format(CultureInfo.InvariantCulture, "{0}_{1}", op.Name, Format.ReservedWord(Format.RemoveDbo(op.Table)))
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

            using (var tw = Format.CreateIndentedTextWriter())
            {
                tw.Write("ALTER TABLE ");
                tw.Write(Format.ReservedWord(Format.RemoveDbo(op.Table)));
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
            using (var tw = Format.CreateIndentedTextWriter())
            {
                tw.Write("DROP TABLE ");
                tw.Write(Format.ReservedWord(Format.RemoveDbo(op.Name)));

                AddSqlStatement(tw);
            }
        }

        private void Generate(SqlOperation opeSQL)
        {
            var sql = Format.RemoveDbo(opeSQL.Sql);

            AddSqlStatement(sql, opeSQL.SuppressTransaction);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "op")]
        private static void Generate(RenameColumnOperation op)
        {
            throw new NotImplementedException("RenameColumn is non-trivial and has not been implemented. See http://sqlite.org/lang_altertable.html");
        }

        private void Generate(RenameTableOperation op)
        {
            using (var tw = Format.CreateIndentedTextWriter())
            {
                tw.Write("ALTER TABLE ");
                tw.Write(Format.ReservedWord(Format.RemoveDbo(op.Name)));
                tw.Write(" RENAME TO ");
                tw.Write(Format.ReservedWord(Format.RemoveDbo(op.NewName)));

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

            tw.Write(Format.ReservedWord(column.Name));
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
                    tw.Write(Format.BuildColumnType(_providerManifest, column));

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
                tw.Write(Format.BuildColumnType(_providerManifest, column));

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
                    tw.Write("(" + column.DefaultValueSql + ")");
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

        #endregion



        #region Support methods

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "tableName")]
        private static void SetAnnotatedColumn(ColumnModel column, string tableName)
        {
            AnnotationValues values;
            if (column.Annotations.TryGetValue("SqlDefaultValue", out values))
            {
                if (values.NewValue == null)
                {
                    column.DefaultValueSql = null;
                    using (var writer = Format.CreateIndentedTextWriter())
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
        /// Generate CREATE TABLE command
        /// </summary>
        private void GenerateCreateTableCommand(CreateTableOperation op, IndentedTextWriter tw)
        {
            tw.WriteLine("CREATE TABLE " + Format.ReservedWord(Format.RemoveDbo(op.Name)) + " (");
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
                tw.Write(Format.RemoveDbo(op.PrimaryKey.Name));
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

        private static OnConflictAction GetUniqueConflictAction(AnnotationValues uniqueAnnotation)
        {
            var uniqueText = Convert.ToString(uniqueAnnotation.NewValue, CultureInfo.InvariantCulture);
            var action = OnConflictAction.None;

            if (uniqueText.StartsWith("OnConflict:", StringComparison.OrdinalIgnoreCase))
            {
                var actionText = uniqueText.Remove(0, "OnConflict:".Length).Trim();
                if (Enum.TryParse(actionText, out action))
                    return action;
            }

            return action;
        }

        #endregion
    }
}
