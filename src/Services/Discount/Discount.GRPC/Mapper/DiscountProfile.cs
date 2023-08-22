using AutoMapper;
using Discount.Entities;
using Discount.GRPC.Protos;

namespace Discount.GRPC.Mapper
{
    public class DiscountProfile : Profile
    {
        public DiscountProfile()
        {
            CreateMap<Coupon, CouponModel>()
            .ForMember(dest => dest.AmountScaled, opt => opt.MapFrom(src => Convert.ToInt64(src.Amount * 100)))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId ?? string.Empty))
            .ReverseMap()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => (decimal)src.AmountScaled / 100.0m));
        }
    }
}
