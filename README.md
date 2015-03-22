# SQLiteEfCodeFirstDbCreator
Creates a SQLite Database based on a EdmModel by using Entity Framework CodeFirst.


This Project ships a "SqliteContextInitializer" which creates a new SQLite Database, based on your model/code.
I started with the code from flaub: https://gist.github.com/flaub/1968486e1b3f2b9fddaf

Currently the following is supported:
- Tables from classes
- Columns from properties
- 1-n relationships
- Cascade on delete
- PrimaryKey constraint
- Not Null constraint

I tried to write the code in a extensible way.
The logic is devided into two main parts. Builder and Statement.
The Builder-Part knows how to translate the EdmModel into statements where a statement class creates the SQLite-SQL-Code.