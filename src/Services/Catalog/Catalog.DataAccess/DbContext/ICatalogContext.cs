using MongoDB.Driver;
using Catalog.Entities;

namespace Catalog.DataAccess.DbContext
{
    public interface ICatalogContext
    {
        IMongoCollection<Product> Products { get; }
    }
}
