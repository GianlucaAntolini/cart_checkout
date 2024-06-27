using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourNamespace.Models;

namespace YourNamespace.Data.Repositories
{
    public class UserOrderRepository : Repository<UserOrder>
    {
        public UserOrderRepository(IUnitOfwork unitOfwork) : base(unitOfwork)
        {
        }


        public async Task<ActionResult<UserOrder>> GetLatestOrderOfUserByUserId(string userId)
        {
            return await dbSet.OrderByDescending(uo => uo.Id).FirstOrDefaultAsync(uo => uo.UserId == userId);
        }


        public async Task<ActionResult<IEnumerable<UserOrder>>> GetAllByUserId(string userId)
        {
            return await dbSet.Where(uo => uo.UserId == userId).ToListAsync();
        }

        public async Task<ActionResult<UserOrder>> GetByOrderIdAndUserId(int orderId, string userId)
        {
            return await dbSet.FirstOrDefaultAsync(uo => uo.OrderId == orderId && uo.UserId == userId);
        }
    }
}