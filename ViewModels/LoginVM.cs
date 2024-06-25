using System.ComponentModel.DataAnnotations;

namespace YourNamespace.ViewModels;

public class LoginVM
{
    [Required(ErrorMessage = "Email obbligatoria.")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Password obbligatoria.")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Display(Name = "Ricordami")]
    public bool RememberMe { get; set; }
}