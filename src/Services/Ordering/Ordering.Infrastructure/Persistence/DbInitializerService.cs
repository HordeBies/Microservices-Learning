using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Application.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Persistence
{
    public class DbInitializerService : IDbInitializerService
    {
        private readonly ILogger<DbInitializerService> logger;
        private readonly OrderingContext context;

        public DbInitializerService(ILogger<DbInitializerService> logger, OrderingContext context)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task Initialize(int retry = 0)
        {
            int retryForAvailability = retry;

            try
            {
                logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(OrderingContext).Name);

                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }
            }
            catch (SqlException e)
            {
                logger.LogError(e, "An error occurred while migrating the database used on context {DbContextName}", typeof(OrderingContext).Name);

                if(retryForAvailability < 50)
                {
                    retryForAvailability++;
                    Task.Delay(2000);
                    Initialize(retryForAvailability);
                }
            }

            return Task.CompletedTask;
        }
    }
}
