using YourNamespace.Models;

namespace YourNamespace.Data.Repositories
{
    public class OrderProductRepository : Repository<OrderProduct>
    {
        public OrderProductRepository(IUnitOfwork unitOfwork) : base(unitOfwork)
        {
        }
    }
}