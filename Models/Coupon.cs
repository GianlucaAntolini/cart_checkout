namespace YourNamespace.Models
{
public class Coupon
{
    public int Id { get; set; }
    public string Code { get; set; }
    public bool Active { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal PercentageDiscount { get; set; }
}

}