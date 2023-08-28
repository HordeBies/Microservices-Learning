using WebApp.Extensions;
using WebApp.Models;

namespace WebApp.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient client;

        public CatalogService(HttpClient client, ILogger<CatalogService> logger)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IEnumerable<CatalogModel>> GetCatalog()
        {
            var response = await client.GetAsync("/Catalog");
            return await response.ReadContentAs<List<CatalogModel>>() ?? new List<CatalogModel>();
        }

        public async Task<CatalogModel?> GetCatalog(string id)
        {
            var response = await client.GetAsync($"/Catalog/{id}");
            return await response.ReadContentAs<CatalogModel>();
        }

        public async Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category)
        {
            var response = await client.GetAsync($"/Catalog/GetProductByCategory/{category}");
            return await response.ReadContentAs<List<CatalogModel>>() ?? new List<CatalogModel>();
        }

        public async Task<CatalogModel?> CreateCatalog(CatalogModel model)
        {
            var response = await client.PostAsJson($"/Catalog", model);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<CatalogModel>();
            else
            {
                throw new Exception("Something went wrong when calling api.");
            }
        }
    }
}
