namespace WebApp.Models
{
    public class BasketModel
    {
        public string UserName { get; set; }
        public string? DiscountCode { get; set; }
        public decimal? DiscountAmount { get; set; } = 0;
        public List<BasketItemModel> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public decimal? DiscountedTotalPrice { get; set; }
    }
}
