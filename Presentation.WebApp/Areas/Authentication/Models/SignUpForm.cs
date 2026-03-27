using System.ComponentModel.DataAnnotations;

namespace Presentation.WebApp.Areas.Authentication.Models;

public class SignUpForm
{
    [Required(ErrorMessage = "Email is required.")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email Adress", Prompt = "username@example.com")]
    public required string Email { get; set; } 

    [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions.")]
    public bool TermsAndConditions { get; set; }
    public string? ErrorMessage { get; set; }

}
