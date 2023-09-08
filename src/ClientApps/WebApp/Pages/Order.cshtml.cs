using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Models;
using WebApp.Services;

namespace WebApp
{
    [Authorize]
    public class OrderModel : PageModel
    {
        private readonly IOrderService orderService;

        public OrderModel(IOrderService orderService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        public IEnumerable<OrderResponseModel> Orders { get; set; } = new List<OrderResponseModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User not found");
            Orders = await orderService.GetOrdersByUserName(userName);

            return Page();
        }       
    }
}