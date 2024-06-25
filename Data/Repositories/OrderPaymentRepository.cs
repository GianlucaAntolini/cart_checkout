using Microsoft.AspNetCore.Mvc;
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

        // Get order payment by order id with executionDate != null and linked payment type
        public async Task<OrderPayment> GetByOrderIdWithExecutionDatAndPaymentType(int orderId)
        {
            return await dbSet.Include(op => op.PaymentType).FirstOrDefaultAsync(op => op.OrderId == orderId && op.ExecutionDate != null);
        }

        // Has order been paid by order id (if there is a order payment with order id and executionDate != null
        public async Task<ActionResult<bool>> HasOrderBeenPaidByOrderId(int id)
        {
            return await dbSet.AnyAsync(op => op.OrderId == id && op.ExecutionDate != null);
        }
    }
}