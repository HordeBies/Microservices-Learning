using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Models;
using WebApp.Services;

namespace WebApp
{
    public class CartModel : PageModel
    {
        private readonly IBasketService basketService;

        public CartModel(IBasketService basketService)
        {
            this.basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
        }

        public BasketModel Cart { get; set; } = new BasketModel();

        [BindProperty] // TODO: Add OnPostApplyCouponAsync method, add coupon field and apply coupon button
        public string? CouponCode { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // TODO: Hard coded userName until identity is implemented
            var userName = "bies";
            Cart = await basketService.GetBasket(userName);

            return Page();
        }
        // TODO: Add OnPostChangeQuantityAsync method, trigger it when quantity field is changed and update the basket
        public async Task<IActionResult> OnPostRemoveToCartAsync(string productId)
        {
            // TODO: Hard coded userName until identity is implemented
            var userName = "bies";
            var basket = await basketService.GetBasket(userName);
            basket.DiscountCode = CouponCode;

            var basketItem = basket.Items.FirstOrDefault(p => p.ProductId == productId);
            if (basketItem != null)
            {
                basket.Items.Remove(basketItem);
            }

            var updatedBasket = await basketService.UpdateBasket(userName,basket);

            return RedirectToPage();
        }
    }
}