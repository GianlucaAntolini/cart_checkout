using YourNamespace.Models;

namespace YourNamespace.Data.Repositories
{
    public class OrderUserDetailRepository : Repository<OrderUserDetail>
    {
        public OrderUserDetailRepository(IUnitOfwork unitOfwork) : base(unitOfwork)
        {
        }
    }
}