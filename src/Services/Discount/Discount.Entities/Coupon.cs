using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Coupon code is required")]
        [MinLength(5, ErrorMessage = "Coupon code must be at least 5 characters long")]
        public string CouponCode { get; set; }

        // TODO: These are for a better coupon system that i do not need currently.
        //public int MaxUsage { get; set; } 
        //public int CurrentUsage { get; set; }
        //public DateTime ExpirationDate { get; set; }
    }
}
