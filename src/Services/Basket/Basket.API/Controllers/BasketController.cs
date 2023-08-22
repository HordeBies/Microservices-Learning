using Basket.DataAccess.Repositories;
using Basket.Entities;
using Basket.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository basketRepository;
        private readonly IDiscountGrpcService discountGrpcService;

        public BasketController(IBasketRepository basketRepository, IDiscountGrpcService discountGrpcService)
        {
            this.basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
            this.discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await basketRepository.GetBasket(userName) ?? new ShoppingCart(userName);
            
            return Ok(basket);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            // TODO: Communicate with Discount.GRPC and get discounted prices for the products in the basket.
            foreach (var item in basket.Items)
            {
                if(item.DiscountCode is not null)
                {
                    var coupon = await discountGrpcService.GetDiscount(item.DiscountCode, item.ProductId);
                    item.DiscountAmount = (coupon.AmountScaled / 100.0m);
                }else
                    item.DiscountAmount = 0;
            }
            if (basket.DiscountCode is not null)
            {
                var coupon = await discountGrpcService.GetDiscount(basket.DiscountCode, null);
                basket.DiscountAmount = (coupon.AmountScaled / 100.0m);
            }else
                basket.DiscountAmount = 0;


            return Ok(await basketRepository.UpdateBasket(basket));
        }

        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteBasket(string userName)
        {
            await basketRepository.DeleteBasket(userName);
            return Ok();
        }
    }
}
