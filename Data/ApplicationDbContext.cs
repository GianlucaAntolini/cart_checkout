using Microsoft.EntityFrameworkCore;

namespace YourNamespace.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //DbSets
        //public DbSet<Product> Products { get; set; }
        //public DbSet<Coupon> Coupons { get; set; }
        //public DbSet<Order> Orders { get; set; }
        //public DbSet<OrderProduct> OrderProducts { get; set; }
        //public DbSet<OrderPayment> OrderPayments { get; set; }
        //public DbSet<PaymentType> PaymentTypes { get; set; }
        //public DbSet<OrderInvoice> OrderInvoices { get; set; }
        //public DbSet<OrderUserDetail> OrderUserDetails { get; set; }
        //public DbSet<NewsletterSubscription> NewsletterSubscriptions { get; set; }
        //public DbSet<Nation> Nations { get; set; }


    }
}
