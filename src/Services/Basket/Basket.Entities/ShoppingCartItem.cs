
namespace Basket.Entities
{
    public class ShoppingCartItem
    {
        public int Quantity { get; set; }
        public string Color { get; set; }
        public decimal UnitPrice { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
