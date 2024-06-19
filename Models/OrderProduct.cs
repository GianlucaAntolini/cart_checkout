namespace YourNamespace.Models
{
    public class OrderProduct : IEntity
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        // The price of the product at the time of the order
        // It is the toal: price * quantity
        public decimal Price { get; set; }
        public Product Product { get; set; }
        public Order Order { get; set; }

    }
}