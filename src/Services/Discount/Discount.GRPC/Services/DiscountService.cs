using AutoMapper;
using Discount.DataAccess.Repositories;
using Discount.Entities;
using Discount.GRPC.Protos;
using Grpc.Core;

namespace Discount.GRPC.Services
{
    public class DiscountService : Protos.DiscountService.DiscountServiceBase
    {
        private readonly ILogger<DiscountService> logger;
        private readonly IDiscountRepository discountRepository;
        private readonly IMapper mapper;
        public DiscountService(ILogger<DiscountService> logger, IDiscountRepository discountRepository, IMapper mapper)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.discountRepository = discountRepository ?? throw new ArgumentNullException(nameof(discountRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await discountRepository.GetDiscount(request.CouponCode, request.ProductId);
            if(coupon is null || coupon.Id == -1)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with CouponCode={request.CouponCode} and ProductId={request.ProductId} is not found."));
            }
            return mapper.Map<CouponModel>(coupon);
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            if ((await discountRepository.GetDiscount(request.Coupon.CouponCode, request.Coupon.ProductId)).Id != -1)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, $"Discount with CouponCode={request.Coupon.CouponCode} and ProductId={request.Coupon.ProductId} already exists."));
            }

            var success = await discountRepository.CreateDiscount(mapper.Map<Coupon>(request.Coupon));
            if (success)
                logger.LogInformation($"Discount with CouponCode={request.Coupon.CouponCode} and ProductId={request.Coupon.ProductId} is created.");
            else
                logger.LogError($"Failed to create discount with CouponCode={request.Coupon.CouponCode} and ProductId={request.Coupon.ProductId}.");

            var coupon = await discountRepository.GetDiscount(request.Coupon.CouponCode, request.Coupon.ProductId);

            return mapper.Map<CouponModel>(coupon);
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            if ((await discountRepository.GetDiscount(request.Coupon.CouponCode, request.Coupon.ProductId)).Id != -1)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, $"Discount with CouponCode={request.Coupon.CouponCode} and ProductId={request.Coupon.ProductId} already exists."));
            }

            var success = await discountRepository.UpdateDiscount(mapper.Map<Coupon>(request.Coupon));
            if (success)
                logger.LogInformation($"Discount with CouponCode={request.Coupon.CouponCode} and ProductId={request.Coupon.ProductId} is updated.");
            else
                logger.LogError($"Failed to update discount with CouponCode={request.Coupon.CouponCode} and ProductId={request.Coupon.ProductId}.");

            var coupon = await discountRepository.GetDiscount(request.Coupon.CouponCode, request.Coupon.ProductId);

            return mapper.Map<CouponModel>(coupon);
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var deleted = await discountRepository.DeleteDiscount(request.CouponCode, request.ProductId);
            if(deleted)
                logger.LogInformation($"Discount with CouponCode={request.CouponCode} and ProductId={request.ProductId} is deleted.");
            else
                logger.LogError($"Failed to delete discount with CouponCode={request.CouponCode} and ProductId={request.ProductId}.");

            return new DeleteDiscountResponse { Success = deleted };
        }
    }
}
