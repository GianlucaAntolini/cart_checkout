using YourNamespace.Models;

namespace YourNamespace.Data.Repositories
{
    public class OrderPaymentRepository : Repository<OrderPayment>
    {
        public OrderPaymentRepository(IUnitOfwork unitOfwork) : base(unitOfwork)
        {
        }
    }
}