using System.Data.Entity.Core.Metadata.Edm;

namespace SQLite.CodeFirst
{
    public interface ISqlGenerator
    {
        string Generate(EdmModel storeModel);
    }
}