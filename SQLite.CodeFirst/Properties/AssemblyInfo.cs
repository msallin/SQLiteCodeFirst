using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("SQLite.CodeFirst")]
[assembly: AssemblyDescription(
    "Creates a SQLite Database from Code, using Entity Framework CodeFirst. " +
    "This Project ships several IDbInitializer which creates " +
    "a new SQLite Database, based on your model/code.")]
[assembly: AssemblyProduct("SQLite.CodeFirst")]
[assembly: AssemblyCopyright("Copyright © Marc Sallin")]
[assembly: AssemblyCompany("Marc Sallin")]

[assembly: InternalsVisibleTo("SQLite.CodeFirst.Test", AllInternalsVisible = true)]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("46603902-448a-4c50-87ec-09cb792b740f")]

// Will be replaced by the build server
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0.0")]
