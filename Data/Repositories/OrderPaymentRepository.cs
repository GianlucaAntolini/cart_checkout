using Microsoft.EntityFrameworkCore;
using YourNamespace.Models;

namespace YourNamespace.Data.Repositories
{
    public class OrderPaymentRepository : Repository<OrderPayment>
    {
        public OrderPaymentRepository(IUnitOfwork unitOfwork) : base(unitOfwork)
        {
        }


        // Get order payment by order id with the highest id (meaning the latest payment attempt)
        public async Task<OrderPayment> GetByOrderIdWithHighestId(int orderId)
        {
            return await dbSet.OrderByDescending(op => op.Id).FirstOrDefaultAsync(op => op.OrderId == orderId);
        }
    }
}