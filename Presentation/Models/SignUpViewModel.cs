using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class SignUpViewModel
{
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Full name must be at least 3 characters")]
    [RegularExpression(@"^[A-Za-zÅÄÖåäö]+(?:\s+[A-Za-zÅÄÖåäö]+)+$", ErrorMessage = "Enter your full name (first and last)")]
    [DataType(DataType.Text)]
    [Display(Name = "FullName", Prompt = "Enter your full name")]
    public string FullName { get; set; } = null!;

    [Required]
    [RegularExpression("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$", ErrorMessage = "Invalid email address")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email", Prompt = "Enter your email address")]
    public string Email { get; set; } = null!;

    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$", ErrorMessage = "Password must contain uppercase, lowercase, a number, and a special character")]
    [DataType(DataType.Password)]
    [Display(Name = "Password", Prompt = "Enter password")]
    public string Password { get; set; } = null!;


    [Required]
    [Compare(nameof(Password))]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password", Prompt = "Confirm password")]
    public string ConfirmPassword { get; set; } = null!;

    [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms.")]
    public bool Terms { get; set; }
}