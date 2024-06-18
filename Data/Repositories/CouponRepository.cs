using YourNamespace.Models;

namespace YourNamespace.Data.Repositories
{
    public class CouponRepository : Repository<Coupon>
    {
        public CouponRepository(IUnitOfwork unitOfwork) : base(unitOfwork)
        {
        }
    }
}