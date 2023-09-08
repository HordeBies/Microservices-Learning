using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Models;
using WebApp.Services;

namespace WebApp
{
    [Authorize]
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
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User not found");
            Cart = await basketService.GetBasket(userName);
            Order = new()
            {
                UserName = userName,
                EmailAddress = User.FindFirst(JwtClaimTypes.Email)?.Value ?? throw new Exception("User not found")
            };
            try
            {
                Order.FirstName = User.FindFirst(JwtClaimTypes.GivenName)?.Value;
                Order.LastName = User.FindFirst(JwtClaimTypes.FamilyName)?.Value;
                Order.AddressLine = User.FindFirst(JwtClaimTypes.Address)?.Value;
            }
            catch (Exception)
            {

                throw;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostCheckOutAsync()
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User not found");
            Cart = await basketService.GetBasket(userName);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Order.UserName = userName;
            Order.EmailAddress = User.FindFirst(JwtClaimTypes.Email)?.Value ?? throw new Exception("User not found");
            Order.TotalPrice = Cart.TotalPrice;

            await basketService.CheckoutBasket(userName, Order);

            return RedirectToPage("Confirmation", "OrderSubmitted");
        }
    }
}