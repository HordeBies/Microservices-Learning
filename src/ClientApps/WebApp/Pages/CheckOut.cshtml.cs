using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Models;
using WebApp.Services;

namespace WebApp
{
    public class CheckOutModel : PageModel
    {
        private readonly IBasketService basketService;

        public CheckOutModel(IBasketService basketService)
        {
            this.basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
        }

        [BindProperty]
        public BasketCheckoutModel Order { get; set; }

        public BasketModel Cart { get; set; } = new BasketModel();

        public async Task<IActionResult> OnGetAsync()
        {
            // TODO: Hard coded userName until identity is implemented
            var userName = "bies";
            Cart = await basketService.GetBasket(userName);
            return Page();
        }

        public async Task<IActionResult> OnPostCheckOutAsync()
        {
            // TODO: Hard coded userName until identity is implemented
            var userName = "bies";
            Cart = await basketService.GetBasket(userName);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Order.UserName = userName;
            Order.TotalPrice = Cart.TotalPrice;

            await basketService.CheckoutBasket(userName,Order);
            
            return RedirectToPage("Confirmation", "OrderSubmitted");
        }       
    }
}