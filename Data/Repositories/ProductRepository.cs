using YourNamespace.Models;

namespace YourNamespace.Data.Repositories
{
    public class ProductRepository : Repository<Product>
    {
        public ProductRepository(IUnitOfwork unitOfwork) : base(unitOfwork)
        {
        }

      
    }
}
