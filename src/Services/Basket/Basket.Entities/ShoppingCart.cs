
namespace Basket.Entities
{
    public class ShoppingCart
    {
        public string UserName { get; set; }
        public string? DiscountCode { get; set; }
        // TODO: Create DTOs to secure sensitive data
        public decimal? DiscountAmount { get; set; } = 0;
        public List<ShoppingCartItem> Items { get; set; }
        public decimal TotalPrice => Items.Sum(item => item.TotalPrice);
        public decimal? DiscountedTotalPrice => Items.Sum(item => item.DiscountedTotalPrice) - DiscountAmount;
        public ShoppingCart(string userName)
        {
            this.UserName = userName;
            this.Items = new List<ShoppingCartItem>();
        }
    }
}
