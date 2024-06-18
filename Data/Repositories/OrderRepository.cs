using YourNamespace.Models;

namespace YourNamespace.Data.Repositories
{
    public class OrderRepository : Repository<Order>
    {
        public OrderRepository(IUnitOfwork unitOfwork) : base(unitOfwork)
        {
        }
    }
}