using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Models;
using WebApp.Services;

namespace WebApp
{
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
            // TODO: Hard coded userName until identity is implemented
            var userName = "bies";
            Orders = await orderService.GetOrdersByUserName(userName);

            return Page();
        }       
    }
}