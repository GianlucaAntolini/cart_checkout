using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YourNamespace.Models;

namespace YourNamespace.Data
{
	public class ApplicationDbContext : IdentityDbContext<User>
	{
		protected readonly IConfiguration Configuration;

		public ApplicationDbContext(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
		}

		//DbSets
		public DbSet<Product> Products { get; set; }
		public DbSet<Coupon> Coupons { get; set; }
		public DbSet<CouponProduct> CouponProducts { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderProduct> OrderProducts { get; set; }
		public DbSet<OrderPayment> OrderPayments { get; set; }
		public DbSet<PaymentType> PaymentTypes { get; set; }
		public DbSet<OrderInvoice> OrderInvoices { get; set; }
		public DbSet<OrderUserDetail> OrderUserDetails { get; set; }
		public DbSet<NewsletterSubscription> NewsletterSubscriptions { get; set; }
		public DbSet<Nation> Nations { get; set; }

		public DbSet<UserOrder> UserOrders { get; set; }

		// Foreign keys, indexes and default constraints
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			// User has many UserOrders
			modelBuilder.Entity<User>()
				.HasMany(u => u.UserOrders)
				.WithOne(uo => uo.User)
				.HasForeignKey(uo => uo.UserId);

			//UserOrder has one Order
			modelBuilder.Entity<UserOrder>()
			.HasOne(uo => uo.Order)
			.WithMany()
			.HasForeignKey(uo => uo.OrderId);


			// Order can have one Coupon
			modelBuilder.Entity<Order>()
				.HasOne(o => o.Coupon)
				.WithMany()
				.HasForeignKey(o => o.CouponId);

			// Order has many OrderProducts
			modelBuilder.Entity<Order>()
				.HasMany(o => o.OrderProducts)
				.WithOne(op => op.Order)
				.HasForeignKey(op => op.OrderId);


			// OrderProduct has one Product
			modelBuilder.Entity<OrderProduct>()
				.HasOne(op => op.Product)
				.WithMany()
				.HasForeignKey(op => op.ProductId);

			// OrderProduct has one Order
			//modelBuilder.Entity<OrderProduct>()
			//  .HasOne(op => op.Order)
			// .WithMany()
			//.HasForeignKey(op => op.OrderId);

			// Order has one OrderPayment
			modelBuilder.Entity<Order>()
				.HasOne(o => o.OrderPayment)
				.WithOne(op => op.Order)
				.HasForeignKey<OrderPayment>(op => op.OrderId);

			// Order has one OrderInvoice
			modelBuilder.Entity<Order>()
				.HasOne(o => o.OrderInvoice)
				.WithOne(oi => oi.Order)
				.HasForeignKey<OrderInvoice>(oi => oi.OrderId);

			// Order has max one OrderUserDetail
			modelBuilder.Entity<Order>()
				.HasOne(o => o.OrderUserDetail)
				.WithOne(oud => oud.Order)
				.HasForeignKey<OrderUserDetail>(oud => oud.OrderId);

			// OrderUserDetail has one Order
			modelBuilder.Entity<OrderUserDetail>()
				.HasOne(oud => oud.Order)
				.WithOne(o => o.OrderUserDetail)
				.HasForeignKey<OrderUserDetail>(oud => oud.OrderId);


			// OrderUserDetail has one Nation
			modelBuilder.Entity<OrderUserDetail>()
				.HasOne(oud => oud.Nation)
				.WithMany()
				.HasForeignKey(oud => oud.NationId);

			// OrderPayment has one PaymentType
			modelBuilder.Entity<OrderPayment>()
				.HasOne(op => op.PaymentType)
				.WithMany()
				.HasForeignKey(op => op.PaymentTypeId);


			// Coupon has many CouponProducts
			modelBuilder.Entity<Coupon>()
				.HasMany(c => c.CouponProducts)
				.WithOne(cp => cp.Coupon)
				.HasForeignKey(cp => cp.CouponId);

			// CouponProduct has one Product
			modelBuilder.Entity<CouponProduct>()
				.HasOne(cp => cp.Product)
				.WithMany()
				.HasForeignKey(cp => cp.ProductId);


		}

	}
}
