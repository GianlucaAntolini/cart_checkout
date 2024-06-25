﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using YourNamespace.Data;

#nullable disable

namespace cart_checkout.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("YourNamespace.Models.Coupon", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("MaxPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("MinPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("PercentageDiscount")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("Coupons");
                });

            modelBuilder.Entity("YourNamespace.Models.CouponProduct", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CouponId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CouponId");

                    b.HasIndex("ProductId");

                    b.ToTable("CouponProducts");
                });

            modelBuilder.Entity("YourNamespace.Models.Nation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Nations");
                });

            modelBuilder.Entity("YourNamespace.Models.NewsletterSubscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("SubscriptionDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("NewsletterSubscriptions");
                });

            modelBuilder.Entity("YourNamespace.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CouponId")
                        .HasColumnType("int");

                    b.Property<DateTime>("OrderTimestamp")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Privacy")
                        .HasColumnType("bit");

                    b.Property<bool>("TermsAndConditions")
                        .HasColumnType("bit");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TotalAmountWithCoupon")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("CouponId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("YourNamespace.Models.OrderInvoice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CF")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<string>("PIVA")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("OrderInvoices");
                });

            modelBuilder.Entity("YourNamespace.Models.OrderPayment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CancelDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ExecutionDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<int>("PaymentTypeId")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.HasIndex("PaymentTypeId");

                    b.ToTable("OrderPayments");
                });

            modelBuilder.Entity("YourNamespace.Models.OrderProduct", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("PriceWithCoupon")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderProducts");
                });

            modelBuilder.Entity("YourNamespace.Models.OrderUserDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NationId")
                        .HasColumnType("int");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("NationId");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("OrderUserDetails");
                });

            modelBuilder.Entity("YourNamespace.Models.PaymentType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PaymentTypes");
                });

            modelBuilder.Entity("YourNamespace.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("StockQuantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("YourNamespace.Models.CouponProduct", b =>
                {
                    b.HasOne("YourNamespace.Models.Coupon", "Coupon")
                        .WithMany("CouponProducts")
                        .HasForeignKey("CouponId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("YourNamespace.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Coupon");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("YourNamespace.Models.Order", b =>
                {
                    b.HasOne("YourNamespace.Models.Coupon", "Coupon")
                        .WithMany()
                        .HasForeignKey("CouponId");

                    b.Navigation("Coupon");
                });

            modelBuilder.Entity("YourNamespace.Models.OrderInvoice", b =>
                {
                    b.HasOne("YourNamespace.Models.Order", "Order")
                        .WithOne("OrderInvoice")
                        .HasForeignKey("YourNamespace.Models.OrderInvoice", "OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("YourNamespace.Models.OrderPayment", b =>
                {
                    b.HasOne("YourNamespace.Models.Order", "Order")
                        .WithOne("OrderPayment")
                        .HasForeignKey("YourNamespace.Models.OrderPayment", "OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("YourNamespace.Models.PaymentType", "PaymentType")
                        .WithMany()
                        .HasForeignKey("PaymentTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("PaymentType");
                });

            modelBuilder.Entity("YourNamespace.Models.OrderProduct", b =>
                {
                    b.HasOne("YourNamespace.Models.Order", "Order")
                        .WithMany("OrderProducts")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("YourNamespace.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("YourNamespace.Models.OrderUserDetail", b =>
                {
                    b.HasOne("YourNamespace.Models.Nation", "Nation")
                        .WithMany()
                        .HasForeignKey("NationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("YourNamespace.Models.Order", "Order")
                        .WithOne("OrderUserDetail")
                        .HasForeignKey("YourNamespace.Models.OrderUserDetail", "OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Nation");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("YourNamespace.Models.Coupon", b =>
                {
                    b.Navigation("CouponProducts");
                });

            modelBuilder.Entity("YourNamespace.Models.Order", b =>
                {
                    b.Navigation("OrderInvoice")
                        .IsRequired();

                    b.Navigation("OrderPayment")
                        .IsRequired();

                    b.Navigation("OrderProducts");

                    b.Navigation("OrderUserDetail")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}