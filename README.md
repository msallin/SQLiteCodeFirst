# SQLite CodeFirst
**Release Build** [![Build status](https://ci.appveyor.com/api/projects/status/2qavdqctw0ehscm6/branch/master?svg=true)](https://ci.appveyor.com/project/msallin/sqlitecodefirst-nv6vn/branch/master)

**CI Build** [![Build status](https://ci.appveyor.com/api/projects/status/oc1miog385h801qe?svg=true)](https://ci.appveyor.com/project/msallin/sqlitecodefirst)

Creates a [SQLite Database](https://sqlite.org/) from Code, using [Entity Framework](https://msdn.microsoft.com/en-us/data/ef.aspx) CodeFirst.

This Project ships several `IDbInitializer` which creates a new SQLite Database, based on your model/code.
I started with the [code](https://gist.github.com/flaub/1968486e1b3f2b9fddaf) from [flaub](https://github.com/flaub). 

Currently the following is supported:
- Tables from classes (supported annotations: `Table`)
- Columns from properties (supported annotations: `Column`, `Key`, `MaxLength`, `Required`, `NotMapped`, `DatabaseGenerated`)
- PrimaryKey constraint (`Key` annotation, key composites are supported)
- ForeignKey constraint (1-n relationships, support for 'Cascade on delete')
- Not Null constraint
- Auto increment (An int PrimaryKey will automatically be incremented)

I tried to write the code in a extensible way.
The logic is divided into two main parts. Builder and Statement.
The Builder knows how to translate the EdmModel into statements where a statement class creates the SQLite-DDL-Code. 
The structure of the statements is influenced by the [SQLite Language Specification](https://www.sqlite.org/lang.html).
You will find an extensive usage of the composite pattern.

## How to use
Either get the assembly from the latest [release](https://github.com/msallin/SQLiteCodeFirst/releases) or install the NuGet-Package [SQLite.CodeFirst](https://www.nuget.org/packages/SQLite.CodeFirst/).

If you want to let the Entity Framework create the database, if it does not exist, just set `SqliteDropCreateDatabaseAlways<>` or `SqliteCreateDatabaseIfNotExists<>` as your `IDbInitializer<>`.
```csharp
public class MyDbContext : DbContext
{
    public MyDbContext()
        : base("ConnectionStringName") { }
  
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<MyDbContext>(
            Database.Connection.ConnectionString, modelBuilder);
        Database.SetInitializer(sqliteConnectionInitializer);
    }
}
```

In a more advanced scenario you may want to populate some core- or test-data after the database was created.
To do this, inherit from `SqliteDropCreateDatabaseAlways<>` or `SqliteCreateDatabaseIfNotExists<>` and override the `Seed(MyDbContext context)` function.
This function will be called, in a own transaction, right after the database was created. This function is only executed if a new database was successfully created.
```csharp
public class MyDbContextContextInitializer : SqliteDropCreateDatabaseAlways<MyDbContext>
{
    public MyDbContextInitializer(string connectionString, DbModelBuilder modelBuilder)
        : base(connectionString, modelBuilder) { }

    protected override void Seed(MyDbContext context)
    {
        context.Set<Player>().Add(new Player());
    }
}
```

## Hints
If you try to reinstall the NuGet-Packages (e.g. if you want to downgrade to .NET 4.0) the app.config will be overwritten and you may getting an exception when you try to run the console project.
In this case please check the following issue: https://github.com/msallin/SQLiteCodeFirst/issues/13.
