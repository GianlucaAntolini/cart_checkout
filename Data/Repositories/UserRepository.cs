using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourNamespace.Models;

namespace YourNamespace.Data.Repositories
{
    public class UserRepository : Repository<Order>
    {
        public UserRepository(IUnitOfwork unitOfwork) : base(unitOfwork)
        {
        }


    }
}