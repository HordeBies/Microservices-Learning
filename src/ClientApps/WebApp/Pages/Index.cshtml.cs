using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICatalogService catalogService;
        private readonly IBasketService basketService;

        public IndexModel(ICatalogService catalogService, IBasketService basketService)
        {
            this.catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
            this.basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
        }

        public IEnumerable<CatalogModel> ProductList { get; set; } = new List<CatalogModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            ProductList = await catalogService.GetCatalog();
            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(string productId, string? CouponCode)
        {
            // TODO: Hard coded userName until identity is implemented
            var userName = "bies";
            var product = await catalogService.GetCatalog(productId);

            var basket = await basketService.GetBasket(userName);
            var basketProduct = basket.Items.FirstOrDefault(p => p.ProductId == product.Id);
            if (basketProduct != null)
            {
                basketProduct.ProductName = product.Name;
                basketProduct.UnitPrice = product.Price;
                basketProduct.Quantity++;
                basketProduct.Color = "Black";
            }
            else
            {
                basket.Items.Add(new BasketItemModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = 1,
                    DiscountCode = CouponCode,
                    Color = "black"
                });
            }

            var updatedBasket = await basketService.UpdateBasket(userName, basket);

            return RedirectToPage("Cart");
        }
    }
}
