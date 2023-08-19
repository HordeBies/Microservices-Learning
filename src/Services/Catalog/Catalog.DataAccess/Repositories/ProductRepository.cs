using Catalog.DataAccess.DbContext;
using Catalog.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.DataAccess.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ICatalogContext context;
        public ProductRepository(ICatalogContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CreateProduct(Product product)
        {
            await context.Products.InsertOneAsync(product);
        }

        public async Task<bool> DeleteProduct(string id)
        {
            var result = await context.Products.DeleteOneAsync(p => p.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await context.Products.Find(p => true).ToListAsync();
        }

        public async Task<Product?> GetProduct(string id)
        {
            return await context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(string category)
        {
            //FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Category, category);
            return await context.Products.Find(p => p.Category == category).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByName(string name)
        {
            //FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Name, name);
            return await context.Products.Find(p => p.Name == name /*filter*/).ToListAsync();
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            var result = await context.Products.ReplaceOneAsync(p => p.Id == product.Id, product);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
