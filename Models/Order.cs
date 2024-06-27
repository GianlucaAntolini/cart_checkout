namespace YourNamespace.Models
{
    public class Order : IEntity
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalAmountWithCoupon { get; set; }
        public DateTime OrderTimestamp { get; set; }
        public bool Privacy { get; set; }
        public bool TermsAndConditions { get; set; }
        public int? CouponId { get; set; }

        public Coupon Coupon { get; set; }
        // This is needed since we can't just put the products in the order, we need OrderProduct for the quantity
        public ICollection<OrderProduct> OrderProducts { get; set; }

        public OrderPayment OrderPayment { get; set; }
        public OrderInvoice OrderInvoice { get; set; }
        public OrderUserDetail OrderUserDetail { get; set; }
    }





}

