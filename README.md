# SQLite CodeFirst
**Release Build** [![Build status](https://ci.appveyor.com/api/projects/status/2qavdqctw0ehscm6/branch/master?svg=true)](https://ci.appveyor.com/project/msallin/sqlitecodefirst-nv6vn/branch/master)

**CI Build** [![Build status](https://ci.appveyor.com/api/projects/status/oc1miog385h801qe?svg=true)](https://ci.appveyor.com/project/msallin/sqlitecodefirst)

**Support** <a href="https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=ARTMHALNW4VC6&lc=CH&item_name=SQLite%2eCodeFirst&item_number=sqlitecodefirst&currency_code=CHF&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted" title="Donate to this project using Paypal"><img src="https://camo.githubusercontent.com/11b2f47d7b4af17ef3a803f57c37de3ac82ac039/68747470733a2f2f696d672e736869656c64732e696f2f62616467652f70617970616c2d646f6e6174652d79656c6c6f772e737667" alt="PayPal donate button" data-canonical-src="https://img.shields.io/badge/paypal-donate-yellow.svg" style="max-width:100%;"></a>

To support this project you can: report bugs by creating new issues, write code and create PRs or donate.
Especially if you use it for a commercial project, a donation is welcome.


Creates a [SQLite Database](https://sqlite.org/) from Code, using [Entity Framework](https://msdn.microsoft.com/en-us/data/ef.aspx) CodeFirst.

This Project ships several `IDbInitializer` classes. These create new SQLite Databases based on your model/code.
I started with the [code](https://gist.github.com/flaub/1968486e1b3f2b9fddaf) from [flaub](https://github.com/flaub). 

Currently the following is supported:
- Tables from classes (supported annotations: `Table`)
- Columns from properties (supported annotations: `Column`, `Key`, `MaxLength`, `Required`, `NotMapped`, `DatabaseGenerated`, `Index`)
- PrimaryKey constraint (`Key` annotation, key composites are supported)
- ForeignKey constraint (1-n relationships, support for 'Cascade on delete')
- Not Null constraint
- Auto increment (An int PrimaryKey will automatically be incremented and you can explicit set the "AUTOINCREMENT" constraint to a PrimaryKey using the Autoincrement-Attribute)
- Index (Decorate columns with the `Index` attribute. Indices are automatically created for foreign keys by default. To prevent this you can remove the convetion `ForeignKeyIndexConvention`)
- Unique constraint (Decorate columns with the `UniqueAttribute`, which is part of this library)
- Collate constraint (Decorate columns with the `CollateAttribute`, which is part of this library)

I tried to write the code in a extensible way.
The logic is divided into two main parts, Builder and Statement.
The Builder knows how to translate the EdmModel into statements where a statement class creates the SQLite-DDL-Code. 
The structure of the statements is influenced by the [SQLite Language Specification](https://www.sqlite.org/lang.html).
You will find an extensive usage of the composite pattern.

## Install
Either get the assembly from the latest [GitHub Release Page](https://github.com/msallin/SQLiteCodeFirst/releases) or install the NuGet-Package [SQLite.CodeFirst](https://www.nuget.org/packages/SQLite.CodeFirst/) (`PM> Install-Package SQLite.CodeFirst`).

The project is built to target.NET framework versions 4.0 and 4.5.
You can use the SQLite CodeFirst in projects that target the following frameworks:
- .NET 4.0 (use net40)
- .NET 4.5 (use net45)
- .NET 4.5.1 (use net45)
- .NET 4.5.2 (use net45)
- .NET 4.6 (use net45)
- .NET 4.6.1 (use net45)

## How to use
The functionality is exposed by using implementations of the `IDbInitializer<>` interface.
Depending on your need, you can choose from the following initializers:
- SqliteCreateDatabaseIfNotExists 
- SqliteDropCreateDatabaseAlways
- SqliteDropCreateDatabaseWhenModelChanges

If you want to have more control, you can use the `SqliteDatabaseCreator` (implements `IDatabaseCreator`) which lets you control the creation of the SQLite database.
Or for even more control, use the `SqliteSqlGenerator` (implements `ISqlGenerator`), which lets you generate the SQL code based on your `EdmModel`.

When you want to let the Entity Framework create database if it does not exist, just set `SqliteDropCreateDatabaseAlways<>` or `SqliteCreateDatabaseIfNotExists<>` as your `IDbInitializer<>`.

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
This table is used to store some information to detect model changes. If you want to use a own entity/table you can implement the
`IHistory` interface and pass the type of your entity as parameter in the to the constructor from the initializer. 

In a more advanced scenario, you may want to populate some core- or test-data after the database was created.
To do this, inherit from `SqliteDropCreateDatabaseAlways<>`, `SqliteCreateDatabaseIfNotExists<>` or `SqliteDropCreateDatabaseWhenModelChanges<>` and override the `Seed(MyDbContext context)` function.
This function will be called in a transaction once the database was created.  This function is only executed if a new database was successfully created.
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

## Hints
If you try to reinstall the NuGet-Packages (e.g. if you want to downgrade to .NET 4.0), the app.config will be overwritten and you may getting an exception when you try to run the console project.
In this case please check the following issue: https://github.com/msallin/SQLiteCodeFirst/issues/13.
