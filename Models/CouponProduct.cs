namespace YourNamespace.Models
{
    public class CouponProduct : IEntity
{
    public int Id { get; set; }
    public int CouponId { get; set; }
    public int ProductId { get; set; }

    public Coupon Coupon { get; set; }
    public Product Product { get; set;  }

}
}