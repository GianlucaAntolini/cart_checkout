namespace YourNamespace.Models
{
    public class OrderProduct : IEntity
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        // The price of the product at the time of the order
        // It is the total: price * quantity
        public decimal Price { get; set; }
        // The price of the product at the time of the order with the coupon applied
        // It is the total: price * quantity + coupon discount (capped to max price of the coupon)
        public decimal PriceWithCoupon { get; set; }
        public Product Product { get; set; }
        public Order Order { get; set; }

    }
}