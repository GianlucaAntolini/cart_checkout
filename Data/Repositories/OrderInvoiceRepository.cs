using YourNamespace.Models;

namespace YourNamespace.Data.Repositories
{
    public class OrderInvoiceRepository : Repository<OrderInvoice>
    {
        public OrderInvoiceRepository(IUnitOfwork unitOfwork) : base(unitOfwork)
        {
        }
    }
}