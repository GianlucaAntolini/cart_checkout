namespace YourNamespace.Models
{
public class Product : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool Active { get; set; }
}
}