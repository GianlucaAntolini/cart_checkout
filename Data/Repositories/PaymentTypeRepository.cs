using YourNamespace.Models;

namespace YourNamespace.Data.Repositories
{
    public class PaymentTypeRepository : Repository<PaymentType>
    {
        public PaymentTypeRepository(IUnitOfwork unitOfwork) : base(unitOfwork)
        {
        }
    }
}