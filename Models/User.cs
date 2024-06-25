using Microsoft.AspNetCore.Identity;
using YourNamespace.Models;

public class User : IdentityUser
{
    public ICollection<UserOrder> UserOrders { get; set; }

}