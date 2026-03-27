using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.Areas.Authentication.Models;

public class SetPasswordForm
{
    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password", Prompt = "Enter Password")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Password must be confirmed.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password", Prompt = "Confirm Password")]
    public required string ConfirmPassword { get; set; }
    public string? ErrorMessage { get; set; }

}
