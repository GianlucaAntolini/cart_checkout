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
            return await dbSet.Include(o => o.OrderProducts).ThenInclude(op => op.Product).Include(o => o.Coupon).ThenInclude(c => c.CouponProducts).ThenInclude(cp => cp.Product).Include(o => o.OrderUserDetail).ThenInclude(oud => oud.Nation).Include(o => o.OrderInvoice).FirstOrDefaultAsync(o => o.Id == id);
        }



    }
}