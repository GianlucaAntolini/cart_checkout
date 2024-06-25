using System.ComponentModel.DataAnnotations;

namespace YourNamespace.ViewModels;

public class RegisterVM
{

    [Required]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Compare("Password", ErrorMessage = "Le passord fornite non corrispondono.")]
    [Display(Name = "Conferma Password")]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; set; }
}