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
            // no need to include anything else, as we are only interested in the userorder itself (the one with the highest order id for the user)
            return await dbSet.OrderByDescending(uo => uo.Id).FirstOrDefaultAsync(uo => uo.UserId == userId);
        }

    }
}