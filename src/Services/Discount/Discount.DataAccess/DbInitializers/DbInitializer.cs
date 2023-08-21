using Dapper;
using Discount.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discount.DataAccess.DbInitializers
{
    public class DbInitializer : IDbInitializer
    {
        private readonly DatabaseOptions databaseOptions;
        private readonly ILogger<DbInitializer> logger;

        public DbInitializer(IOptions<DatabaseOptions> databaseOptions, ILogger<DbInitializer> logger)
        {
            this.databaseOptions = databaseOptions.Value ?? throw new ArgumentNullException(nameof(databaseOptions));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Initialize(int retry = 0)
        {
            var retryForAvailability = retry;
            try
            {
                logger.LogInformation("Initializing database");
                using var connection = new NpgsqlConnection(databaseOptions.PostgreSqlConnectionString);
                connection.Open();

                using var command = new NpgsqlCommand
                {
                    Connection = connection
                };
                command.CommandText = "CREATE TABLE IF NOT EXISTS Coupon ( Id SERIAL PRIMARY KEY, ProductId VARCHAR(24), Description TEXT, Amount MONEY, CouponCode VARCHAR(24));";
                command.ExecuteNonQuery();
                var coupons = await connection.QueryAsync("select * from coupon limit 1");
                if (!coupons.Any())
                {
                    command.CommandText = "INSERT INTO Coupon(productid, description, amount, couponcode) VALUES ('602d2149e773f2a3990b47f5', 'IPhone-X Seeded Discount Coupon', 150, 'IPHONE150'),('602d2149e773f2a3990b47f6', 'Samsung 10 Seeded Discount Coupon', 100, 'SAMSUNG100');";
                    command.ExecuteNonQuery();
                }

                logger.LogInformation("Database initialized");
            }
            catch (NpgsqlException ex)
            {
                logger.LogError(ex, "An error occurred while migrating or seeding the database");
                if (retryForAvailability < 5)
                {
                    retryForAvailability++;
                    System.Threading.Thread.Sleep(2000);
                    await Initialize(retryForAvailability);
                }
            }
        }
    }
}
