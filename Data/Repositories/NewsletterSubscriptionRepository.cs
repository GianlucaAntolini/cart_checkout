using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourNamespace.Models;

namespace YourNamespace.Data.Repositories
{
    public class NewsletterSubscriptionRepository : Repository<NewsletterSubscription>
    {
        public NewsletterSubscriptionRepository(IUnitOfwork unitOfwork) : base(unitOfwork)
        {
        }


        // Update by email

        public async Task<ActionResult<NewsletterSubscription>> CreateOrUpdateByEmail(string email, NewsletterSubscription newsletterSubscription)
        {
            var existingNewsletterSubscription = await dbSet.FirstOrDefaultAsync(ns => ns.Email == email);
            if (existingNewsletterSubscription == null)
            {
                // insert new newsletter subscription
                return await Create(newsletterSubscription);
            }

            existingNewsletterSubscription.Email = newsletterSubscription.Email;
            existingNewsletterSubscription.SubscriptionDate = newsletterSubscription.SubscriptionDate;
            existingNewsletterSubscription.Active = newsletterSubscription.Active;

            return existingNewsletterSubscription;
        }
    }
}