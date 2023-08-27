using AutoMapper;
using Basket.DataAccess.Repositories;
using Basket.Entities;
using Basket.Services;
using EventBus.Messages.Events;
using MassTransit;
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
        private readonly IMapper mapper;
        private readonly IPublishEndpoint publishEndpoint;

        public BasketController(IBasketRepository basketRepository, IDiscountGrpcService discountGrpcService, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            this.basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
            this.discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await basketRepository.GetBasket(userName) ?? new ShoppingCart(userName);
            
            return Ok(basket);
        }

        [HttpPost("{userName}", Name = "UpdateBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket(string userName,[FromBody] ShoppingCart basket)
        {
            if (userName != basket.UserName)
                return BadRequest();

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

        [HttpPost("{userName}/[action]")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Accepted)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> Checkout(string userName, [FromBody] BasketCheckout basketCheckout)
        {
            if (userName != basketCheckout.UserName)
                return BadRequest();
            // get existing basket with total price 
            var basket = await basketRepository.GetBasket(basketCheckout.UserName);
            if (basket is null)
                return BadRequest();

            // Create basketCheckoutEvent -- Set TotalPrice on basketCheckout eventMessage
            // TODO: Add basket details to basketCheckout eventMessage
            var eventMessage = mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;

            // send checkout event to rabbitmq
            await publishEndpoint.Publish(eventMessage);

            // remove the basket
            await basketRepository.DeleteBasket(basket.UserName);

            return Accepted();
        }

    }
}
