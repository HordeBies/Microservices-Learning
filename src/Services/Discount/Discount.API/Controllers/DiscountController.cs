using Discount.DataAccess.Repositories;
using Discount.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Discount.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountRepository discountRepository;

        public DiscountController(IDiscountRepository discountRepository)
        {
            this.discountRepository = discountRepository ?? throw new ArgumentNullException(nameof(discountRepository));
        }

        //[HttpGet("{couponCode}", Name ="GetGlobalDiscount")]
        //[HttpGet("{couponCode}/{productId}", Name ="GetProductDiscount")]
        [HttpGet("{couponCode}", Name ="GetDiscount")]
        [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Coupon>> GetDiscount(string couponCode, [FromQuery] string? productId)
        {
            var coupon = await discountRepository.GetDiscount(couponCode, productId);
            return Ok(coupon);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<bool>> CreateDiscount([FromBody] Coupon coupon)
        {
            if ((await discountRepository.GetDiscount(coupon.CouponCode, coupon.ProductId)).Id != -1)
                ModelState.AddModelError("CouponCode", "Coupon already exists.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            _ = await discountRepository.CreateDiscount(coupon);
            return Ok(await discountRepository.GetDiscount(coupon.CouponCode, coupon.ProductId));
        }
        [HttpPut]
        [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<bool>> UpdateDiscount([FromBody] Coupon coupon)
        {
            if ((await discountRepository.GetDiscount(coupon.CouponCode, coupon.ProductId)).Id != -1)
                ModelState.AddModelError("CouponCode", "Coupon already exists.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _ = await discountRepository.UpdateDiscount(coupon);
            return Ok(await discountRepository.GetDiscount(coupon.CouponCode, coupon.ProductId));
        }

        // TODO: Instead of deleting just make it unusable. Better for logging, monitoring and analyzing.
        // This requires better design of copuon validation.
        //[HttpDelete("{couponCode}", Name = "DeleteGlobalDiscount")]
        //[HttpDelete("{couponCode}/{productId}", Name = "DeleteProductDiscount")]
        [HttpDelete("{couponCode}", Name = "DeleteDiscount")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> DeleteDiscount(string couponCode, [FromQuery]string? productId)
        {
            var deleted = await discountRepository.DeleteDiscount(couponCode, productId);
            return Ok(deleted);
        }
    }
}
