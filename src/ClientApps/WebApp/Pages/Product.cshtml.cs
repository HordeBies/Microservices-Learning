using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Models;
using WebApp.Services;

namespace WebApp
{
    public class ProductModel : PageModel
    {
        private readonly ICatalogService catalogService;
        private readonly IBasketService basketService;

        public ProductModel(ICatalogService catalogService, IBasketService basketService)
        {
            this.catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
            this.basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
        }

        public IEnumerable<string> CategoryList { get; set; } = new List<string>();
        public IEnumerable<CatalogModel> ProductList { get; set; } = new List<CatalogModel>();


        [BindProperty(SupportsGet = true)]
        public string SelectedCategory { get; set; }

        public async Task<IActionResult> OnGetAsync(string? categoryName)
        {
            var productList = await catalogService.GetCatalog();
            CategoryList = productList.Select(x => x.Category).ToList();

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                ProductList = await catalogService.GetCatalogByCategory(categoryName);
                SelectedCategory = categoryName;
            }
            else
            {
                ProductList = await catalogService.GetCatalog();
            }

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