using YourNamespace.Models;

namespace YourNamespace.Data.Repositories
{
    public class NewsletterSubscriptionRepository : Repository<NewsletterSubscription>
    {
        public NewsletterSubscriptionRepository(IUnitOfwork unitOfwork) : base(unitOfwork)
        {
        }
    }
}