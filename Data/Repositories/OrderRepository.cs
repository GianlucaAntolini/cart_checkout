using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourNamespace.Models;

namespace YourNamespace.Data.Repositories
{
    public class OrderRepository : Repository<Order>
    {
        public OrderRepository(IUnitOfwork unitOfwork) : base(unitOfwork)
        {
        }

        // Get order by id with related entities
        public async Task<ActionResult<Order>> GetByIdWithRelatedEntities(int id)
        {
            // For now just return the order with the given id and order products
            return await dbSet.Include(o => o.OrderProducts).FirstOrDefaultAsync(o => o.Id == id);

        }
    }
}