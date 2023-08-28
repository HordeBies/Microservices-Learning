using Shopping.Aggregator.Extensions;
using Shopping.Aggregator.Models;

namespace Shopping.Aggregator.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient client;

        public OrderService(HttpClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<IEnumerable<OrderResponseModel>> GetOrdersByUserName(string userName)
        {
            var response = await client.GetAsync($"api/v1/Order/{userName}");
            return await response.ReadContentAs<List<OrderResponseModel>>() ?? new List<OrderResponseModel>();
        }
    }
}
