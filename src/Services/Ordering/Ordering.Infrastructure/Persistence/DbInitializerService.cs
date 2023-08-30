using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Application.ServiceContracts;
using Polly;
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

        public Task Initialize()
        {
            logger.LogInformation("Initializing database");

            var retryPolicy = Policy
                .Handle<Exception>() // SqlException
                .WaitAndRetry(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, timeSpan, retry, ctx) =>
                {
                    logger.LogError(exception, "[{prefix}] Exception {ExceptionType} detected. Retrying for #{retry}", "OrderingDb Initialization", exception?.GetType()?.Name, retry);
                }
            );

            retryPolicy.Execute(() =>
            {
                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }
            });

            logger.LogInformation("Database initialized");
            return Task.CompletedTask;
        }
    }
}
