using Microsoft.AspNetCore.Mvc;
using Shopping.Aggregator.Models;
using Shopping.Aggregator.Services;
using System.Net;

namespace Shopping.Aggregator.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ShoppingController : ControllerBase
    {
        private readonly ICatalogService catalogService;
        private readonly IBasketService basketService;
        private readonly IOrderService orderService;

        public ShoppingController(ICatalogService catalogService, IBasketService basketService, IOrderService orderService)
        {
            this.catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
            this.basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [ProducesResponseType(typeof(ShoppingModel), (int)HttpStatusCode.OK)]
        [HttpGet("{userName}", Name ="GetShopping")]
        public async Task<ActionResult<ShoppingModel>> Get(string userName)
        {
            var basket = await basketService.GetBasket(userName);

            for (int i = 0; i < basket.Items.Count; i++)
            {
                var basketItem = basket.Items[i];
                // catalog item should not be null as we are getting basket item from it
                var catalogItem = (await catalogService.GetCatalog(basketItem.ProductId))!;
                basketItem.ProductName = catalogItem.Name;
                basketItem.Category = catalogItem.Category;
                basketItem.Summary = catalogItem.Summary;
                basketItem.Description = catalogItem.Description;
                basketItem.ImageFile = catalogItem.ImageFile;
            }

            var orders = await orderService.GetOrdersByUserName(userName);

            var shoppingModel = new ShoppingModel
            {
                UserName = userName,
                BasketWithProducts = basket,
                Orders = orders
            };
            return Ok(shoppingModel);
        }
    }
}
