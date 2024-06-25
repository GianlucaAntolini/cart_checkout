
namespace YourNamespace.Models
{
    public class UserOrder : IEntity
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public Order Order { get; set; }

    }
}