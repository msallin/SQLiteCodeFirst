using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using SQLite.CodeFirst.Statement;

namespace SQLite.CodeFirst.Builder
{
    internal class PrimaryKeyStatementBuilder : IStatementBuilder<PrimaryKeyStatement>
    {
        private readonly IEnumerable<EdmMember> keyMembers;

        public PrimaryKeyStatementBuilder(IEnumerable<EdmMember> keyMembers)
        {
            this.keyMembers = keyMembers;
        }

        public PrimaryKeyStatement BuildStatement()
        {
            return new PrimaryKeyStatement(keyMembers.Select(km => km.Name));
        }
    }
}
