namespace YourNamespace.Models
{
    public class OrderUserDetail : IEntity
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public int NationId { get; set; }
        public Nation Nation { get; set; }

        public Order Order { get; set; }
    }
}
