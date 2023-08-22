namespace Basket.Entities
{
    public class ShoppingCartItem
    {
        public int Quantity { get; set; }
        public string Color { get; set; }
        public decimal UnitPrice { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string? DiscountCode { get; set; }
        // TODO: Create DTOs to secure sensitive data
        public decimal? DiscountAmount { get; set; } = 0;
        public decimal TotalPrice => Quantity * UnitPrice;
        public decimal? DiscountedTotalPrice => TotalPrice - DiscountAmount;
    }
}
