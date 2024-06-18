using YourNamespace.Models;

namespace YourNamespace.Data.Repositories
{
    public class CouponProductRepository : Repository<CouponProduct>
    {
        public CouponProductRepository(IUnitOfwork unitOfwork) : base(unitOfwork)
        {
        }
    }
}