
namespace Basket.Entities
{
    public class ShoppingCart
    {
        public string UserName { get; set; }
        public List<ShoppingCartItem> Items { get; set; }
        public decimal TotalPrice => Items.Sum(item => item.Quantity * item.UnitPrice);
        public ShoppingCart(string userName)
        {
            this.UserName = userName;
            this.Items = new List<ShoppingCartItem>();
        }
    }
}
