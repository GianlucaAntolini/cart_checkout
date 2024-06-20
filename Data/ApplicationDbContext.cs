using Microsoft.EntityFrameworkCore;
using YourNamespace.Models;

namespace YourNamespace.Data
{
	public class ApplicationDbContext : DbContext
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

		/*
-- Foreign keys drop
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Orders_Coupons')
ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_Coupons]

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrderProducts_Orders')
ALTER TABLE [OrderProducts] DROP CONSTRAINT [FK_OrderProducts_Orders]

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrderProducts_Products')
ALTER TABLE [OrderProducts] DROP CONSTRAINT [FK_OrderProducts_Products]

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_CouponsProducts_Coupons')
ALTER TABLE [CouponsProducts] DROP CONSTRAINT [FK_CouponsProducts_Coupons]

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_CouponsProducts_Products')
ALTER TABLE [CouponsProducts] DROP CONSTRAINT [FK_CouponsProducts_Products]

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrderPayments_Orders')
ALTER TABLE [OrderPayments] DROP CONSTRAINT [FK_OrderPayments_Orders]

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrderPayments_PaymentTypes')
ALTER TABLE [OrderPayments] DROP CONSTRAINT [FK_OrderPayments_PaymentTypes]

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrderInvoices_Orders')
ALTER TABLE [OrderInvoices] DROP CONSTRAINT [FK_OrderInvoices_Orders]

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrderUserDetails_Orders')
ALTER TABLE [OrderUserDetails] DROP CONSTRAINT [FK_OrderUserDetails_Orders]

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrderUserDetails_Nations')
ALTER TABLE [OrderUserDetails] DROP CONSTRAINT [FK_OrderUserDetails_Nations]

-- Indexes drop
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UniqueCode_Coupons')
DROP INDEX [IX_UniqueCode_Coupons] ON [Coupons]

-- Default constraints drop
IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_Coupons_Active')
ALTER TABLE [Coupons] DROP CONSTRAINT [DF_Coupons_Active]

IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_Coupons_MinPrice')
ALTER TABLE [Coupons] DROP CONSTRAINT [DF_Coupons_MinPrice]

IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_Products_StockQuantity')
ALTER TABLE [Products] DROP CONSTRAINT [DF_Products_StockQuantity]

IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_Products_Active')
ALTER TABLE [Products] DROP CONSTRAINT [DF_Products_Active]

IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_Orders_Privacy')
ALTER TABLE [Orders] DROP CONSTRAINT [DF_Orders_Privacy]

IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_Orders_TermsAndConditions')
ALTER TABLE [Orders] DROP CONSTRAINT [DF_Orders_TermsAndConditions]

IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_OrderPayments_ExecutionDate')
ALTER TABLE [OrderPayments] DROP CONSTRAINT [DF_OrderPayments_ExecutionDate]

IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_OrderPayments_CancelDate')
ALTER TABLE [OrderPayments] DROP CONSTRAINT [DF_OrderPayments_CancelDate]



-- Tables drop: TODO, check if exists and drop and check correct order
IF OBJECT_ID('Coupons', 'U') IS NOT NULL
DROP TABLE Coupons

IF OBJECT_ID('Products', 'U') IS NOT NULL
DROP TABLE Products

IF OBJECT_ID('CouponsProducts', 'U') IS NOT NULL
DROP TABLE CouponsProducts

IF OBJECT_ID('Nations', 'U') IS NOT NULL
DROP TABLE Nations

IF OBJECT_ID('NewsletterSubscription', 'U') IS NOT NULL
DROP TABLE NewsletterSubscription

IF OBJECT_ID('Orders', 'U') IS NOT NULL
DROP TABLE Orders

IF OBJECT_ID('OrderProducts', 'U') IS NOT NULL
DROP TABLE OrderProducts

IF OBJECT_ID('OrderPayments', 'U') IS NOT NULL
DROP TABLE OrderPayments

IF OBJECT_ID('PaymentTypes', 'U') IS NOT NULL
DROP TABLE PaymentTypes

IF OBJECT_ID('OrderInvoices', 'U') IS NOT NULL
DROP TABLE OrderInvoices

IF OBJECT_ID('OrderUserDetails', 'U') IS NOT NULL
DROP TABLE OrderUserDetails

IF OBJECT_ID('Nations', 'U') IS NOT NULL
DROP TABLE Nations

-- Tables creation
	-- Coupons
CREATE TABLE [Coupons] (
	[Id] [int] IDENTITY(1,1) NOT NULL, -- Coupon id
	[Code] [nvarchar](10) NOT NULL, -- Coupon code
	[Active] [bit] NOT NULL, -- Is the coupon active?
	[MinPrice] [decimal](18, 2) NOT NULL, -- Minimum price to apply the coupon
	[MaxPrice] [decimal](18, 2) NULL, -- Maximum price to apply the coupon
	[PercentageDiscount] [decimal](18, 2) NULL, -- Percentage discount
 	CONSTRAINT [PK_Coupons] PRIMARY KEY  
	(
		[Id] ASC
	)
)

	-- Products
CREATE TABLE [Products] (
	[Id] [int] IDENTITY(1,1) NOT NULL, -- Product id
	[Name] [nvarchar](max) NULL, -- Product name
	[Price] [decimal](18, 2) NOT NULL, -- Product price x unit
	[StockQuantity] [int] NOT NULL, -- Product stock quantity available
	[Active] [bit] NOT NULL, -- Is the product active?
 	CONSTRAINT [PK_Products] PRIMARY KEY
	(
		[Id] ASC
	)
)

	-- Products that are discountable by a coupon
CREATE TABLE [CouponsProducts] (
	[Id] [int] IDENTITY(1,1) NOT NULL, -- Coupon product id
	[CouponId] [int] NOT NULL, -- Coupon id
	[ProductId] [int] NOT NULL, -- Product id
 	CONSTRAINT [PK_CouponsProducts] PRIMARY KEY
	(
		[Id] ASC
	)
)

	-- Nations
CREATE TABLE [Nations] (
	[Id] [int] IDENTITY(1,1) NOT NULL, -- Nation id
	[Name] [nvarchar](max) NULL, -- Nation name
 	CONSTRAINT [PK_Nations] PRIMARY KEY
	(
		[Id] ASC
	)
)

	-- Newsletter subscriptions
CREATE TABLE [NewsletterSubscription] (
	[Id] [int] IDENTITY(1,1) NOT NULL, -- Subscription id
	[Email] [nvarchar](max) NOT NULL, -- Subscription email
	[SubscriptionDate] [datetime] NOT NULL, -- Subscription date
	[Active] [bit] NOT NULL, -- Is the subscription active?
 	CONSTRAINT [PK_NewsletterSubscription] PRIMARY KEY
	(
		[Id] ASC
	)
)

	-- Orders
CREATE TABLE [Orders] (
	[Id] [int] IDENTITY(1,1) NOT NULL, -- Order id
	[TotalAmount] [decimal](18, 2) NOT NULL, -- Total amount
	[TotalAmountWithCoupon] [decimal](18, 2) NOT NULL, -- Total amount with coupon applied
	[OrderTimestamp] [datetime] NOT NULL, -- Order timestamp
	[Privacy] [bit] NOT NULL, -- Privacy flag --> GDPR required?
	[TermsAndConditions] [bit] NOT NULL, -- Terms and conditions flag --> GDPR required?
	[CouponId] [int] NULL, -- Coupon id
 	CONSTRAINT [PK_Orders] PRIMARY KEY
	(
		[Id] ASC
	)
)

	-- Products in an order
CREATE TABLE [OrderProducts] (
	[Id] [int] IDENTITY(1,1) NOT NULL, -- Order product id
	[OrderId] [int] NOT NULL, -- Order id
	[ProductId] [int] NOT NULL, -- Product id
	[Quantity] [int] NOT NULL, -- Product quantity --> at the moment of the order
	[Price] [decimal](18, 2) NOT NULL, -- Total pice of the product in the order --> quantity * product price
	[PriceWithCoupon] [decimal](18, 2) NOT NULL, -- Total price of the product in the order with coupon applied --> this is useful since the coupon applies to single products and is calculated also with min and max price
 	CONSTRAINT [PK_OrderProducts] PRIMARY KEY
	(
		[Id] ASC
	)
)


	-- Order payments
CREATE TABLE [OrderPayments] (
	[Id] [int] IDENTITY(1,1) NOT NULL, -- Order payment id
	[OrderId] [int] NOT NULL, -- Order id
	[TotalAmount] [decimal](18, 2) NOT NULL, -- Total amount
	[ExecutionDate] [datetime] NULL, -- Execution date --> when the payment is executed it is filled
	[CancelDate] [datetime] NULL, -- Cancel date --> when the payment is canceled it is filled
	[PaymentTypeId] [int] NOT NULL, -- Payment type id
 	CONSTRAINT [PK_OrderPayments] PRIMARY KEY
	(
		[Id] ASC
	)
)

	-- Payment types
CREATE TABLE [PaymentTypes] (
	[Id] [int] IDENTITY(1,1) NOT NULL, -- Payment type id
	[Name] [nvarchar](max) NOT NULL, -- Payment type name
 	CONSTRAINT [PK_PaymentTypes] PRIMARY KEY
	(
		[Id] ASC
	)
)

	-- Orders invoices
CREATE TABLE [OrderInvoices] (
	[Id] [int] IDENTITY(1,1) NOT NULL, -- Invoice id
	[OrderId] [int] NOT NULL, -- Order id
	[PIVA] [nvarchar](max) NOT NULL, -- PIVA
	[CF] [nvarchar](max) NOT NULL, -- CF
 	CONSTRAINT [PK_OrderInvoices] PRIMARY KEY
	(
		[Id] ASC
	)
)		
	-- Orders user details
CREATE TABLE [OrderUserDetails] (
	[Id] [int] IDENTITY(1,1) NOT NULL, -- User details id
	[OrderId] [int] NOT NULL, -- Order id
	[Name] [nvarchar](max) NOT NULL, -- User name
	[Surname] [nvarchar](max) NOT NULL, -- User surname
	[Email] [nvarchar](max) NOT NULL, -- User email
	[NationId] [int] NOT NULL, -- Nation id
 	CONSTRAINT [PK_OrderUserDetails] PRIMARY KEY
	(
		[Id] ASC
	)
)



-- Foreign keys
ALTER TABLE [Orders] ADD  CONSTRAINT [FK_Orders_Coupons] FOREIGN KEY([CouponId])
REFERENCES [Coupons] ([Id])

ALTER TABLE [OrderProducts] ADD  CONSTRAINT [FK_OrderProducts_Orders] FOREIGN KEY([OrderId])
REFERENCES [Orders] ([Id])

ALTER TABLE [OrderProducts] ADD  CONSTRAINT [FK_OrderProducts_Products] FOREIGN KEY([ProductId])
REFERENCES [Products] ([Id])

ALTER TABLE [CouponsProducts] ADD  CONSTRAINT [FK_CouponsProducts_Coupons] FOREIGN KEY([CouponId])
REFERENCES [Coupons] ([Id])

ALTER TABLE [CouponsProducts] ADD  CONSTRAINT [FK_CouponsProducts_Products] FOREIGN KEY([ProductId])
REFERENCES [Products] ([Id])

ALTER TABLE [OrderPayments] ADD  CONSTRAINT [FK_OrderPayments_Orders] FOREIGN KEY([OrderId])
REFERENCES [Orders] ([Id])

ALTER TABLE [OrderPayments] ADD  CONSTRAINT [FK_OrderPayments_PaymentTypes] FOREIGN KEY([PaymentTypeId])
REFERENCES [PaymentTypes] ([Id])

ALTER TABLE [OrderInvoices] ADD  CONSTRAINT [FK_OrderInvoices_Orders] FOREIGN KEY([OrderId])
REFERENCES [Orders] ([Id])

ALTER TABLE [OrderUserDetails] ADD  CONSTRAINT [FK_OrderUserDetails_Orders] FOREIGN KEY([OrderId])
REFERENCES [Orders] ([Id])

ALTER TABLE [OrderUserDetails] ADD  CONSTRAINT [FK_OrderUserDetails_Nations] FOREIGN KEY([NationId])
REFERENCES [Nations] ([Id])


-- Indexes and constraints
CREATE UNIQUE NONCLUSTERED INDEX [IX_UniqueCode_Coupons] ON [Coupons]
(
	[Code] ASC
)

ALTER TABLE [Coupons] ADD  CONSTRAINT [DF_Coupons_Active]  DEFAULT ((1)) FOR [Active]

ALTER TABLE [Coupons] ADD  CONSTRAINT [DF_Coupons_MinPrice]  DEFAULT ((0)) FOR [MinPrice]

ALTER TABLE [Products] ADD  CONSTRAINT [DF_Products_StockQuantity]  DEFAULT ((0)) FOR [StockQuantity]

ALTER TABLE [Products] ADD  CONSTRAINT [DF_Products_Active]  DEFAULT ((1)) FOR [Active]

ALTER TABLE [Orders] ADD  CONSTRAINT [DF_Orders_Privacy]  DEFAULT ((0)) FOR [Privacy]

ALTER TABLE [Orders] ADD  CONSTRAINT [DF_Orders_TermsAndConditions]  DEFAULT ((0)) FOR [TermsAndConditions]

ALTER TABLE [OrderPayments] ADD  CONSTRAINT [DF_OrderPayments_ExecutionDate]  DEFAULT (NULL) FOR [ExecutionDate]

ALTER TABLE [OrderPayments] ADD  CONSTRAINT [DF_OrderPayments_CancelDate]  DEFAULT (NULL) FOR [CancelDate]




-- Data inserts
	-- Nations
INSERT INTO [Nations] (Name) VALUES ('Italy')
INSERT INTO [Nations] (Name) VALUES ('USA')
INSERT INTO [Nations] (Name) VALUES ('England')
     -- Payment types
INSERT INTO [PaymentTypes] (Name) VALUES ('Stripe')
INSERT INTO [PaymentTypes] (Name) VALUES ('Paypal')
	 -- Products
INSERT INTO [Products] (Name, Price, StockQuantity, Active) VALUES ('Product 1', 5.00, 3, 1)
INSERT INTO [Products] (Name, Price, StockQuantity, Active) VALUES ('Product 2', 20.00, 10, 1)
INSERT INTO [Products] (Name, Price, StockQuantity, Active) VALUES ('Product 3', 30.00, 0, 1)
INSERT INTO [Products] (Name, Price, StockQuantity, Active) VALUES ('Product 4', 20.00, 5, 0)
INSERT INTO [Products] (Name, Price, StockQuantity, Active) VALUES ('Product 5', 10.00, 15, 1)
	 -- Coupons
INSERT INTO [Coupons] (Code, Active, MinPrice, MaxPrice, PercentageDiscount) VALUES ('COUPON1', 1, 10.00, 100.00, 10.00)
INSERT INTO [Coupons] (Code, Active, MinPrice, MaxPrice, PercentageDiscount) VALUES ('COUPON2', 0, 20.00, 100.00, 20.00)
INSERT INTO [Coupons] (Code, Active, MinPrice, MaxPrice, PercentageDiscount) VALUES ('COUPON3', 1, 30.00, 100.00, 30.00)
	 -- CouponsProducts
INSERT INTO [CouponsProducts] (CouponId, ProductId) VALUES (1, 1)
INSERT INTO [CouponsProducts] (CouponId, ProductId) VALUES (1, 2)
INSERT INTO [CouponsProducts] (CouponId, ProductId) VALUES (2, 3)
INSERT INTO [CouponsProducts] (CouponId, ProductId) VALUES (2, 4)
INSERT INTO [CouponsProducts] (CouponId, ProductId) VALUES (3, 5)
	 





        */
		// Foreign keys, indexes and default constraints
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
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
