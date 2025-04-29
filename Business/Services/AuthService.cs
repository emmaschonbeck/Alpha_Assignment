using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Business.Services;

public interface IAuthService
{
    Task<AuthResult> SignInAsync(SignInFormData formData);
    Task<AuthResult> SignOutAsync();
    Task<AuthResult> SignUpAsync(SignUpFormData formData);
}

public class AuthService(IUserService userService, SignInManager<UserEntity> signInManager, UserManager<UserEntity> userManager) : IAuthService
{
    private readonly IUserService _userService = userService;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly UserManager<UserEntity> _userManager = userManager;

    public async Task<AuthResult> SignInAsync(SignInFormData formData)
    {
        if (formData == null)
            return new AuthResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

        var user = await _userManager.FindByEmailAsync(formData.Email);
        if (user == null)
            return new AuthResult { Succeeded = false, StatusCode = 404, Error = "User not found." };

        var passwordValid = await _userManager.CheckPasswordAsync(user, formData.Password);
        if (!passwordValid)
            return new AuthResult { Succeeded = false, StatusCode = 401, Error = "Invalid email or password." };

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.FullName ?? user.Email)
    };

        var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        await _signInManager.Context.SignInAsync(
            IdentityConstants.ApplicationScheme,
            claimsPrincipal,
            new AuthenticationProperties { IsPersistent = formData.IsPersistent });

        return new AuthResult { Succeeded = true, StatusCode = 200 };
    }

    public async Task<AuthResult> SignUpAsync(SignUpFormData formData)
    {
        if (formData == null)
            return new AuthResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

        var result = await _userService.CreateUserAsync(formData);
        return result.Succeeded
            ? new AuthResult { Succeeded = true, StatusCode = 201 }
            : new AuthResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }

    public async Task<AuthResult> SignOutAsync()
    {
        await _signInManager.SignOutAsync();
        return new AuthResult { Succeeded = true, StatusCode = 200 };
    }
}
