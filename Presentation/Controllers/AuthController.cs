using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class AuthController(IAuthService authService) : Controller
{
    private readonly IAuthService _authService = authService;

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
            return View(model);

        var formData = new SignUpFormData
        {
            FullName = model.FullName,
            Email = model.Email,
            Password = model.Password
        };

        var result = await _authService.SignUpAsync(formData);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Something went wrong when signing up");
            return View(model);
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
            ModelState.AddModelError(string.Empty, result.Error ?? "Invalid email or password");
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
}
