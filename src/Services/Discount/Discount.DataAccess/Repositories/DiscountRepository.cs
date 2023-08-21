using Dapper;
using Discount.Entities;
using Discount.Utilities;
using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discount.DataAccess.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly DatabaseOptions databaseOptions;

        public DiscountRepository(IOptions<DatabaseOptions> databaseOptions)
        {
            this.databaseOptions = databaseOptions.Value;
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(databaseOptions.PostgreSqlConnectionString);
            var affected = await connection.ExecuteAsync( 
                "INSERT INTO Coupon (ProductId, Description, Amount, CouponCode) VALUES (@ProductId, @Description, @Amount, @CouponCode)",
                new { coupon.ProductId, coupon.Description, coupon.Amount, coupon.CouponCode });

            return affected > 0;
        }
        public async Task<bool> DeleteDiscount(string couponCode, string? productId)
        {
            using var connection = new NpgsqlConnection(databaseOptions.PostgreSqlConnectionString);
            var query = productId is null ? "DELETE FROM Coupon WHERE CouponCode = @CouponCode AND ProductId IS NULL" : "DELETE FROM Coupon WHERE CouponCode = @CouponCode AND ProductId = @ProductId";
            var affected = await connection.ExecuteAsync(
                query,
                new { CouponCode = couponCode, ProductId = productId });

            return affected > 0;
        }
        public async Task<Coupon> GetDiscount(string couponCode, string? productId)
        {
            using var connection = new NpgsqlConnection(databaseOptions.PostgreSqlConnectionString);
            var query = productId is null? "SELECT * FROM Coupon WHERE CouponCode = @CouponCode AND ProductId IS NULL" : "SELECT * FROM Coupon WHERE CouponCode = @CouponCode AND ProductId = @ProductId";
            //var all = await connection.QueryAsync<Coupon>("select * from coupon");
            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>(
                query,
                new { CouponCode = couponCode, ProductId = productId });

            return coupon ?? new Coupon { Amount = 0, CouponCode = "", Description = "Invalid Coupon", Id = -1, ProductId = ""}; // or return null coupon and let client handle invalid coupon
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(databaseOptions.PostgreSqlConnectionString);
            var affected = await connection.ExecuteAsync(
                "UPDATE Coupon SET ProductId = @ProductId, Description = @Description, Amount = @Amount, CouponCode = @CouponCode WHERE Id = @Id",
                new { coupon.ProductId, coupon.Description, coupon.Amount, coupon.CouponCode, coupon.Id });

            return affected > 0;
        }
    }
}
