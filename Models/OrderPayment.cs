namespace YourNamespace.Models
{
   public class OrderPayment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime? ExecutionDate { get; set; }
    public DateTime? CancelDate { get; set; }
    public int PaymentTypeId { get; set; }
    public PaymentType PaymentType { get; set; }
}
}