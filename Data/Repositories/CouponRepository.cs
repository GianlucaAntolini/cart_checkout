using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourNamespace.Models;

namespace YourNamespace.Data.Repositories
{
    public class CouponRepository : Repository<Coupon>
    {
        public CouponRepository(IUnitOfwork unitOfwork) : base(unitOfwork)
        {
        }

        // Get coupon by code with related products passing through coupon products
        public async Task<ActionResult<Coupon>> GetByCodeWithProducts(string code)
        {
            return await dbSet.Include(c => c.CouponProducts).ThenInclude(cp => cp.Product).FirstOrDefaultAsync(c => c.Code == code);
        }
    }
}