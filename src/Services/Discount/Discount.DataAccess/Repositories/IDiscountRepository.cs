using Discount.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discount.DataAccess.Repositories
{
    public interface IDiscountRepository
    {
        Task<Coupon> GetDiscount(string couponCode,string? productId);
        Task<bool> CreateDiscount(Coupon coupon);
        Task<bool> UpdateDiscount(Coupon coupon);
        Task<bool> DeleteDiscount(string couponCode, string? productId);
    }
}
