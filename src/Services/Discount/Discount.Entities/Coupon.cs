using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discount.Entities
{
    public class Coupon
    {
        public int? Id { get; set; }
        public string? ProductId { get; set; } // if product id not set its global coupon, else its product coupon
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string CouponCode { get; set; }

        // TODO: These are for a better coupon system that i do not need currently.
        //public int MaxUsage { get; set; } 
        //public int CurrentUsage { get; set; }
        //public DateTime ExpirationDate { get; set; }
    }
}
