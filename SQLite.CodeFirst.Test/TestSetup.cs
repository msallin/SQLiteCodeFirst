using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite.CodeFirst.NetCore.Console;

namespace SQLite.CodeFirst.Test
{
    /// <summary>
    /// Registers the SQLite Entity Framework 6 provider once for the whole test assembly.
    /// On .NET (Core) there is no app.config based provider discovery, so the code based
    /// <see cref="Configuration"/> from the demo project is applied before any test runs.
    /// Setting it explicitly (instead of relying on assembly scanning) makes the registration
    /// independent of which DbContext the test runner happens to touch first.
    /// </summary>
    [TestClass]
    public static class TestSetup
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            DbConfiguration.SetConfiguration(new Configuration());
        }
    }
}
