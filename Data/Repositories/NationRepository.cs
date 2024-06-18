using YourNamespace.Models;

namespace YourNamespace.Data.Repositories
{
    public class NationRepository : Repository<Nation>
    {
        public NationRepository(IUnitOfwork unitOfwork) : base(unitOfwork)
        {
        }
    }
}