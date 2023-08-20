using Basket.Entities;
using Basket.Utility.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basket.DataAccess.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache cache;

        public BasketRepository(IDistributedCache cache)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<ShoppingCart?> GetBasket(string userName)
        {
            var basket = await cache.GetAsync<ShoppingCart>(userName);
            return basket;
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            await cache.SetAsync<ShoppingCart>(basket.UserName, basket);
            return (await GetBasket(basket.UserName))!;
        }

        public async Task DeleteBasket(string userName)
        {
            await cache.RemoveAsync(userName);
        } 

    }
}
