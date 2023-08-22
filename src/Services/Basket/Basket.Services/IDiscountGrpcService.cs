using Discount.GRPC.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basket.Services
{
    public interface IDiscountGrpcService
    {
        public Task<CouponModel> GetDiscount(string CouponCode, string? ProductId);
    }
}
