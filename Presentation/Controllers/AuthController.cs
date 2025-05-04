using Business.Models;
using Business.Services;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class AuthController(IAuthService authService, UserManager<UserEntity> userManager) : Controller
{
    private readonly IAuthService _authService = authService;
    private readonly UserManager<UserEntity> _userManager = userManager;

    [Route("auth/signup")]
    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("auth/signup")]
    public async Task<IActionResult> SignUp(SignUpViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("SignUp", model);
        }

        var formData = new SignUpFormData
        {
            FullName = model.FullName,
            Email = model.Email,
            Password = model.Password
        };

        var result = await _authService.SignUpAsync(formData);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(nameof(model.Email), result.Error ?? "Something went wrong when signing up");
            return View("SignUp", model);
        }

        return RedirectToAction("Login", "Auth");
    }

    [Route("auth/login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("auth/login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var formData = new SignInFormData
        {
            Email = model.Email,
            Password = model.Password,
            IsPersistent = model.IsPersistent
        };

        var result = await _authService.SignInAsync(formData);

        if (!result.Succeeded)
        {
            if (result.StatusCode == 404)
                ModelState.AddModelError(nameof(LoginViewModel.Email), result.Error ?? "User not found");
            else if (result.StatusCode == 401)
                ModelState.AddModelError(nameof(LoginViewModel.Password), result.Error ?? "Invalid password");
            else
                ModelState.AddModelError(string.Empty, result.Error ?? "Login failed");

            return View(model);
        }

        return RedirectToAction("Index", "Projects");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("auth/signout")]
    public async Task<IActionResult> SignOut()
    {
        await _authService.SignOutAsync();
        return RedirectToAction("Login", "Auth");
    }
    

    /*
        ChatGPT 4o - Detta är en API-metod som tar emot en email via query string och kontrollerar ifall email addressen redan finns registrerad.
        Om den redan finns så returnerar metoden { exist: true }, om den inte finns registrerad så returnerar den { exist: false }.
        Denna metoden görs för att förhindra dubletter av email addresser.
    */
    [HttpGet]
    [Route("api/check-email")]
    public async Task<IActionResult> CheckEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email is required.");

        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
            return Ok(new { exists = true });

        return Ok(new { exists = false });
    }
}