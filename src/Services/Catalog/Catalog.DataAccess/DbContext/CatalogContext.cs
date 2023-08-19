using Catalog.Entities;
using Catalog.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Catalog.DataAccess.DbContext
{
    public class CatalogContext : ICatalogContext
    {
        public CatalogContext(IOptions<MongoDbOptions> options)
        {
            MongoDbOptions mongoDbOptions = options.Value!;
            var client = new MongoClient(mongoDbOptions.ConnectionString);
            var database = client.GetDatabase(mongoDbOptions.DatabaseName);
            Products = database.GetCollection<Product>(mongoDbOptions.CollectionName);
            CatalogContextSeed.SeedData(Products).GetAwaiter().GetResult();

        }

        public IMongoCollection<Product> Products { get; }
    }
}
