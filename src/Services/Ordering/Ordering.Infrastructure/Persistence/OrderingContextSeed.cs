using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderingContextSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasData(
                               new Order() { Id = 1, UserName = "bies", FirstName = "Mehmet", LastName = "Demirci", EmailAddress = "oa.mehmetdmrc@gmail.com", AddressLine="AddressLine", Country="Turkey", State="Ankara", ZipCode="06420", CreatedBy="bies", CreatedDate= new DateTime(2023,08,25) });
        }
        //public static async Task SeedAsync(OrderingContext orderContext, ILogger<OrderingContextSeed> logger)
        //{
        //    if (!orderContext.Orders.Any())
        //    {
        //        orderContext.Orders.AddRange(GetPreconfiguredOrders());
        //        await orderContext.SaveChangesAsync();
        //        logger.LogInformation("Seed database associated with context {DbContextName}", typeof(OrderingContext).Name);
        //    }
        //}

        //private static IEnumerable<Order> GetPreconfiguredOrders()
        //{
        //    return new List<Order>
        //    {
        //        new Order() { UserName = "bies", FirstName = "Mehmet", LastName = "Demirci", EmailAddress = "oa.mehmetdmrc@gmail.com"}
        //    };
        //}
    }
}
