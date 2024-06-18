namespace YourNamespace.Models
{
public class NewsletterSubscription
{
    public int Id { get; set; }
    public string Email { get; set; }
    public DateTime SubscriptionDate { get; set; }
    public bool Active { get; set; }
}
}