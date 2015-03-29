# SQLite CodeFirst
Creates a [SQLite Database](https://sqlite.org/) from Code, using [Entity Framework](https://msdn.microsoft.com/en-us/data/ef.aspx) CodeFirst.

This Project ships a `SqliteContextInitializer` which creates a new SQLite Database, based on your model/code.
I started with the [code](https://gist.github.com/flaub/1968486e1b3f2b9fddaf) from [flaub](https://github.com/flaub). 

Currently the following is supported:
- Tables from classes (supported annotations: `Table`)
- Columns from properties (supported annotations: `Column`, `Key`, `MaxLength`, `Required`, `NotMapped`, `DatabaseGenerated`)
- PrimaryKey constraint (`Key` annotation, key composites are supported)
- ForeignKey constraint (1-n relationships, support for 'Cascade on delete')
- Not Null constraint
- Auto increment (An int PrimaryKey will automatically be incremented)

I tried to write the code in a extensible way.
The logic is devided into two main parts. Builder and Statement.
The Builder knows how to translate the EdmModel into statements where a statement class creates the SQLite-DDL-Code. 
The structure of the statements is influenced by the [SQLite Language Specification](https://www.sqlite.org/lang.html).
You will find an extensive usage of the composite pattern.

## How to use
If you want to let the Entity Framework create the database, if it does not exist, just set `SqliteContextInitializer<>` as your `IDbInitializer`.
```csharp
public class MyDbContext : DbContext
{
    public TestDbContext()
        : base("ConnectionStringName") { }
  
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        var sqliteConnectionInitializer = new SqliteContextInitializer<TestDbContext>(
            Database.Connection.ConnectionString, modelBuilder);
        Database.SetInitializer(sqliteConnectionInitializer);
    }
}
```

In a more advanced szenario you may want to populate some core- or test-data after the database was created.
To do this, inherit from `SqliteContextInitializer<>` and override the `Seed(TestDbContext context)` function.
This function will be called, in a own transaction, right after the database was created. This function is only executed if a new database was successfully created.
```csharp
public class TestDbContextInitializer : SqliteContextInitializer<TestDbContext>
{
    public TestDbContextInitializer(string connectionString, DbModelBuilder modelBuilder)
        : base(connectionString, modelBuilder) { }

    protected override void Seed(TestDbContext context)
    {
        context.Set<Player>().Add(new Player());
    }
}
```
