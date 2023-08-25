using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using Ordering.Domain.RepositoryContracts;
using Ordering.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(OrderingContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserName(string userName)
        {
            return await GetAsync(e => e.UserName == userName);
        }
    }
}
