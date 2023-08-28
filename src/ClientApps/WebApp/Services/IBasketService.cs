using WebApp.Models;

namespace WebApp.Services
{
    public interface IBasketService
    {
        Task<BasketModel> GetBasket(string userName);
        Task<BasketModel> UpdateBasket(string userName,BasketModel model);
        Task CheckoutBasket(string userName,BasketCheckoutModel model);
    }
}
