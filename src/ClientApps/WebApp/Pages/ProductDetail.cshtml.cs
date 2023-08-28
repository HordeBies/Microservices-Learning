using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Models;
using WebApp.Services;

namespace WebApp
{
    public class ProductDetailModel : PageModel
    {
        private readonly ICatalogService catalogService;
        private readonly IBasketService basketService;

        public ProductDetailModel(ICatalogService catalogService, IBasketService basketService)
        {
            this.catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
            this.basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
        }

        public CatalogModel? Product { get; set; }

        [BindProperty]
        public string Color { get; set; }

        [BindProperty]
        public int Quantity { get; set; } = 1;

        [BindProperty] // TODO: add OnPostApplyCouponAsync method, add coupon field and apply coupon button
        public string? CouponCode { get; set; }

        public async Task<IActionResult> OnGetAsync(string? productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                return NotFound();
            }

            Product = await catalogService.GetCatalog(productId);
            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(string productId)
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