using WebApp.Extensions;
using WebApp.Models;

namespace WebApp.Services
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient client;

        public BasketService(HttpClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<BasketModel> GetBasket(string userName)
        {
            var response = await client.GetAsync($"/Basket/{userName}");
            return (await response.ReadContentAs<BasketModel>())!;
        }

        public async Task<BasketModel> UpdateBasket(string userName, BasketModel model)
        {
            var response = await client.PostAsJson($"/Basket/{userName}", model);
            if (response.IsSuccessStatusCode)
                return (await response.ReadContentAs<BasketModel>())!;
            else
            {
                throw new Exception("Something went wrong when calling api.");
            }
        }

        public async Task CheckoutBasket(string userName, BasketCheckoutModel model)
        {
            var response = await client.PostAsJson($"/Basket/{userName}/Checkout", model);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Something went wrong when calling api.");
            }
        }
    }
}
