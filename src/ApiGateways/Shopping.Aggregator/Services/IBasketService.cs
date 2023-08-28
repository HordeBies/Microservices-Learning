using Shopping.Aggregator.Models;

namespace Shopping.Aggregator.Services
{
    public interface IBasketService
    {
        Task<BasketModel> GetBasket(string userName);
        Task<BasketModel> UpdateBasket(string userName, BasketModel basket);
    }
}
