# SQLite CodeFirst (With Migrations)
Creates and update a [SQLite Database](https://sqlite.org/) from Code, using [Entity Framework](https://msdn.microsoft.com/en-us/data/ef.aspx) CodeFirst and [Migrations](https://msdn.microsoft.com/pt-br/library/system.data.entity.migrations(v=vs.113).aspx).

## Features
This Project ships several `IDbInitializer` classes. These create new SQLite Databases based on your model/code.

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

The project is built to target .NET framework versions 4.0 and 4.5.
You can use the SQLite CodeFirst in projects that target the following frameworks:
- .NET 4.0 (use net40)
- .NET 4.5 (use net45)
- .NET 4.5.1 (use net45)
- .NET 4.5.2 (use net45)
- .NET 4.6 (use net45)
- .NET 4.6.1 (use net45)
- .NET 4.6.2 (use net45)
- .NET 4.7 (use net45)
- .NET 4.7.1 (use net45)

## How to use
The functionality is exposed by using implementations of the `IDbInitializer<>` interface.
Depending on your need, you can choose from the following initializers:
- SqliteCreateDatabaseIfNotExists 
- SqliteDropCreateDatabaseAlways
- SqliteDropCreateDatabaseWhenModelChanges

If you want to have more control, you can use the `SqliteDatabaseCreator` (implements `IDatabaseCreator`) which lets you control the creation of the SQLite database.
Or for even more control, use the `SqliteSqlGenerator` (implements `ISqlGenerator`), which lets you generate the SQL code based on your `EdmModel`.

When you want to let the Entity Framework create database if it does not exist, just set `SqliteDropCreateDatabaseAlways<>` or `SqliteCreateDatabaseIfNotExists<>` as your `IDbInitializer<>`.

Of course when using Migrations feature you will always use `SqliteCreateDatabaseIfNotExists<>`.

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

## Structure
I tried to write the code in a extensible way.
The logic is divided into two main parts, Builder and Statement.
The Builder knows how to translate the EdmModel into statements where a statement class creates the SQLite-DDL-Code. 
The structure of the statements is influenced by the [SQLite Language Specification](https://www.sqlite.org/lang.html).
You will find an extensive usage of the composite pattern.

## Hints
If you try to reinstall the NuGet-Packages (e.g. if you want to downgrade to .NET 4.0), the app.config will be overwritten and you may getting an exception when you try to run the console project.
In this case please check the following issue: https://github.com/msallin/SQLiteCodeFirst/issues/13.

Pay attention when running Migrations routines because Sqlite does not support some SQL commands suggested by the Entity Framework. For example, to rename a column will be suggested to run `Rename("dbo.table_name", "old_column_name", "new_column_name")`. However Sqlite does not support column rename command!

## Recognition
This project is forked from [msallin](https://github.com/msallin/SQLiteCodeFirst) Sqlite Code First Projet and using [zaniants](https://github.com/zanyants/SQLiteCodeFirst) Sqlite Migrations code.

And [msallin](https://github.com/msallin) started with the [code](https://gist.github.com/flaub/1968486e1b3f2b9fddaf) from [flaub](https://github.com/flaub). 
