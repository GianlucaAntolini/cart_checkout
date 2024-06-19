namespace YourNamespace.Models
{
    public class OrderInvoice : IEntity
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string PIVA { get; set; }
        public string CF { get; set; }

        public Order Order { get; set; }
    }
}