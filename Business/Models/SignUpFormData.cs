namespace Business.Models;

public class SignUpFormData
{
    public string? FullName { get; set; } = null!;
    public string? Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}
