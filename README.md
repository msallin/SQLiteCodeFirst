# SQLite CodeFirst (With Migrations)
Creates and update a [SQLite Database](https://sqlite.org/) from Code, using [Entity Framework](https://msdn.microsoft.com/en-us/data/ef.aspx) CodeFirst and [Migrations](https://msdn.microsoft.com/pt-br/library/system.data.entity.migrations(v=vs.113).aspx).

If you need a specific feature for a commercial project, I am glad to offer a paid implementation.   
This project is forked from [msallin](https://github.com/msallin/SQLiteCodeFirst) Sqlite Code First Projet and [zaniants](https://github.com/zanyants/SQLiteCodeFirst) Sqlite Migrations code.
## Features

This project ships several `IDbInitializer` classes. These create new SQLite Databases based on your model/code.

The following features are supported:

- Tables from classes (supported annotations: `Table`)
- Columns from properties (supported annotations: `Column`, `Key`, `MaxLength`, `Required`, `NotMapped`, `DatabaseGenerated`, `Index`)
- PrimaryKey constraint (`Key` annotation, key composites are supported)
- ForeignKey constraint (1-n relationships, support for 'Cascade on delete')
- Not Null constraint
- Auto increment (An int PrimaryKey will automatically be incremented and you can explicit set the "AUTOINCREMENT" constraint to a PrimaryKey using the Autoincrement-Attribute)
- Index (Decorate columns with the `Index` attribute. Indices are automatically created for foreign keys by default. To prevent this you can remove the convention `ForeignKeyIndexConvention`)
- Unique constraint (Decorate columns with the `UniqueAttribute`, which is part of this library)
- Collate constraint (Decorate columns with the `CollateAttribute`, which is part of this library. Use `CollationFunction.Custom` to specify a own collation function.)
- SQL default value (Decorate columns with the `SqlDefaultValueAttribute`, which is part of this library)

## Install

Either get the assembly from the latest [GitHub Release Page](https://github.com/msallin/SQLiteCodeFirst/releases) or install the NuGet-Package [SQLite.CodeFirst](https://www.nuget.org/packages/SQLite.CodeFirst/) (`PM> Install-Package SQLite.CodeFirst`).

The project is built to target .NET framework versions 4.0 and 4.5 and .NET Standard 2.1.
You can use the SQLite CodeFirst in projects that target the following frameworks:

- .NET 4.0 (uses net40)
- .NET 4.5-4.8 (uses net45)
- .NET Core 3.0-3.1 (uses netstandard2.1)
- .NET 5 (uses netstandard2.1)

## How to use

The functionality is exposed by using implementations of the `IDbInitializer<>` interface.
Depending on your need, you can choose from the following initializers:

- SqliteCreateDatabaseIfNotExists
- SqliteDropCreateDatabaseAlways
- SqliteDropCreateDatabaseWhenModelChanges
- SqliteMigrateDatabaseToLatestVersion

If you want to have more control, you can use the `SqliteDatabaseCreator` (implements `IDatabaseCreator`) which lets you control the creation of the SQLite database.
Or for even more control, use the `SqliteSqlGenerator` (implements `ISqlGenerator`), which lets you generate the SQL code based on your `EdmModel`.

When you want to let the Entity Framework create database if it does not exist, just set `SqliteDropCreateDatabaseAlways<>` or `SqliteCreateDatabaseIfNotExists<>` as your `IDbInitializer<>`.

When using Migrations feature you will always use `SqliteMigrateDatabaseToLatestVersion<>`.

**Know issue with Migrations:**
Entity Framework 6.2.0 has an initialization bug with Sqlite and some other databases. It already [was fixed](https://github.com/aspnet/EntityFramework6/issues/398) to EF 6.3. When you need to create a new migration change using command `Add-Migration` you will need to downgrade EF to version 6.1.3 until EF 6.3 is not released.

### Initializer Sample

```csharp
public class MyDbContext : DbContext
{
    public MyDbContext()
        : base("ConnectionStringName") { }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<MyDbContext>(modelBuilder);
        Database.SetInitializer(sqliteConnectionInitializer);
    }
}
```

Notice that the `SqliteDropCreateDatabaseWhenModelChanges<>` initializer will create a additional table in your database.
This table is used to store some information to detect model changes. If you want to use an own entity/table you have to implement the
`IHistory` interface and pass the type of your entity as parameter to the constructor of the initializer.

In a more advanced scenario, you may want to populate some core- or test-data after the database was created.
To do this, inherit from `SqliteDropCreateDatabaseAlways<>`, `SqliteCreateDatabaseIfNotExists<>` or `SqliteDropCreateDatabaseWhenModelChanges<>` and override the `Seed(MyDbContext context)` function.
This function will be called in a transaction, once the database was created.  This function is only executed if a new database was successfully created.

```csharp
public class MyDbContextInitializer : SqliteDropCreateDatabaseAlways<MyDbContext>
{
    public MyDbContextInitializer(DbModelBuilder modelBuilder)
        : base(modelBuilder) { }

    protected override void Seed(MyDbContext context)
    {
        context.Set<Player>().Add(new Player());
    }
}
```

### SqliteDatabaseCreator Sample

```csharp
public class MyContext : DbContext
{
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        var model = modelBuilder.Build(Database.Connection);
        IDatabaseCreator sqliteDatabaseCreator = new SqliteDatabaseCreator();
        sqliteDatabaseCreator.Create(Database, model);
    }
}
```

### SqliteSqlGenerator Sample

```csharp
public class MyContext : DbContext
{
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        var model = modelBuilder.Build(Database.Connection);
        ISqlGenerator sqlGenerator = new SqliteSqlGenerator();
        string sql = sqlGenerator.Generate(model.StoreModel);
    }
}
```

### .NET Core example

Add the following package references.
```xml
<PackageReference Include="System.Data.SQLite" Version="1.0.112.2" />
<PackageReference Include="System.Data.SQLite.EF6" Version="1.0.112.2" />
```

Add the following class.
```csharp
public Configuration()
{
    SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
    SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);

    var providerServices = (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices));

    SetProviderServices("System.Data.SQLite", providerServices);
    SetProviderServices("System.Data.SQLite.EF6", providerServices);

    SetDefaultConnectionFactory(this);
}

public DbConnection CreateConnection(string connectionString)
    => new SQLiteConnection(connectionString);
}
```

## Structure

The code is written in an extensible way.
The logic is divided into two main parts, Builder and Statement.
The Builder knows how to translate the EdmModel into statements where a statement class creates the SQLite-DDL-Code.
The structure of the statements is influenced by the [SQLite Language Specification](https://www.sqlite.org/lang.html).
You will find an extensive usage of the composite pattern.

## Hints

If you try to reinstall the NuGet-Packages (e.g. if you want to downgrade to .NET 4.0), the app.config will be overwritten and you may getting an exception when you try to run the console project.
In this case please check the following issue: <https://github.com/msallin/SQLiteCodeFirst/issues/13.>

Pay attention when running Migrations routines because Sqlite does not support some SQL commands suggested by the Entity Framework. For example, to rename a column will be suggested to run `Rename("dbo.table_name", "old_column_name", "new_column_name")`. However Sqlite does not support column rename command!

When creating a new Migrations change may you need to inform StringConnection property to Migration, like this:
`Add-Migration MyChangeName -ConnectionString "Data Source='C:\path_to_your_database\footballDb.sqlite';" -ConnectionProviderName "System.Data.SQLite"`

## Recognition
I started with the [code](https://gist.github.com/flaub/1968486e1b3f2b9fddaf) from [flaub](https://github.com/flaub).

Migrations uses a code started by Julio C. Borges and upgraded by [digocesar](https://github.com/digocesar).
