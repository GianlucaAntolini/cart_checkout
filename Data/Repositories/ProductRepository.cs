using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourNamespace.Models;

namespace YourNamespace.Data.Repositories
{
    public class ProductRepository : Repository<Product>
    {
        public ProductRepository(IUnitOfwork unitOfwork) : base(unitOfwork)
        {
        }

        // Get all products that are available (Active = true and StockQuantity > 0)
        public async Task<ActionResult<IEnumerable<Product>>> GetAvailableProducts()
        {
            return await dbSet.Where(p => p.Active && p.StockQuantity > 0).ToListAsync();
        }

        

      
    }
}
