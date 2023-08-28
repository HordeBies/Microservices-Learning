namespace Shopping.Aggregator.Models
{
    public class BasketItemExtendedModel
    {
        public int Quantity { get; set; }
        public string Color { get; set; }
        public decimal UnitPrice { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string? DiscountCode { get; set; }
        public decimal? DiscountAmount { get; set; } = 0;
        public decimal TotalPrice { get; set; }
        public decimal DiscountedTotalPrice { get; set; }

        //Product Related Additional Fields
        public string Category { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string ImageFile { get; set; }
    }
}
