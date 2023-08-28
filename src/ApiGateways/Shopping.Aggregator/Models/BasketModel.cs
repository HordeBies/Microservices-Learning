namespace Shopping.Aggregator.Models
{
    public class BasketModel
    {
        public string UserName { get; set; }
        public string? DiscountCode { get; set; }
        public decimal? DiscountAmount { get; set; } = 0;

        public List<BasketItemExtendedModel> Items { get; set; } = new List<BasketItemExtendedModel>();
        public decimal TotalPrice { get; set; }
        public decimal DiscountedTotalPrice { get; set; }
    }
}
