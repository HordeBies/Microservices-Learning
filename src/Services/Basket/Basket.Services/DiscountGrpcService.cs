using Discount.GRPC.Protos;
namespace Basket.Services
{
    public class DiscountGrpcService : IDiscountGrpcService
    {
        private readonly DiscountService.DiscountServiceClient discountService;

        public DiscountGrpcService(DiscountService.DiscountServiceClient discountService)
        {
            this.discountService = discountService;
        }

        public async Task<CouponModel> GetDiscount(string CouponCode, string? ProductId)
        {
            var discountRequest = new GetDiscountRequest { CouponCode = CouponCode, ProductId = ProductId ?? string.Empty };

            return await discountService.GetDiscountAsync(discountRequest);
        }
    }
}
