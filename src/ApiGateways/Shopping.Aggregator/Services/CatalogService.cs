using Shopping.Aggregator.Extensions;
using Shopping.Aggregator.Models;

namespace Shopping.Aggregator.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient client;

        public CatalogService(HttpClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IEnumerable<CatalogModel>> GetCatalog()
        {
            var response = await client.GetAsync("api/v1/Catalog");
            return await response.ReadContentAs<List<CatalogModel>>() ?? new List<CatalogModel>();
        }

        public async Task<CatalogModel?> GetCatalog(string id)
        {
            var response = await client.GetAsync($"api/v1/Catalog/{id}");
            return await response.ReadContentAs<CatalogModel>();
        }

        public async Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category)
        {
            var response = await client.GetAsync($"api/v1/Catalog/GetProductByCategory/{category}");
            return await response.ReadContentAs<List<CatalogModel>>() ?? new List<CatalogModel>();
        }
    }
}
