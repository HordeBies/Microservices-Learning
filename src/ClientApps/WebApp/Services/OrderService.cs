using WebApp.Extensions;
using WebApp.Models;

namespace WebApp.Services
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
            var response = await client.GetAsync($"/Order/{userName}");
            return await response.ReadContentAs<List<OrderResponseModel>>() ?? new List<OrderResponseModel>();
        }
    }
}
