using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Ordering.Application.Models;
using Ordering.Application.ServiceContracts;
using Ordering.Domain.RepositoryContracts;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Repositories;
using Ordering.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<OrderingContext>(options => options.UseSqlServer(configuration.GetConnectionString("OrderingConnectionString")));
            
            services.AddScoped<IOrderRepository, OrderRepository>();

            services.Configure<EmailSettings>(c => configuration.GetSection("EmailSettings"));
            services.AddTransient<IEmailService, EmailService>();

            return services;
        }
    }
}
