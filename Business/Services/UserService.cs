using Data.Entities;
using Data.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace Business.Models;

public interface IUserService
{
    Task<UserResult> CreateUserAsync(SignUpFormData formData);
    Task<UserResult> GetUsersAsync();
}

public class UserService(IUserRepository userRepository, UserManager<UserEntity> userManager) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly UserManager<UserEntity> _userManager = userManager;

    public async Task<UserResult> GetUsersAsync()
    {
        var result = await _userRepository.GetAllAsync();
        return result.Succeeded
            ? new UserResult { Succeeded = true, StatusCode = result.StatusCode, Result = result.Result }
            : new UserResult { Succeeded = false, Error = "No statuses found." };
    }

    public async Task<UserResult> CreateUserAsync(SignUpFormData formData)
    {
        if (formData == null)
            return new UserResult { Succeeded = false, StatusCode = 400, Error = "form data can't be null." };

        var existResult = await _userRepository.ExistsAsync(x => x.Email == formData.Email);
        if (existResult.Succeeded)
            return new UserResult { Succeeded = false, StatusCode = 409, Error = "user with same email already exists." };

        try
        {
            var userEntity = new UserEntity
            {
                FullName = formData.FullName,
                Email = formData.Email,
                UserName = formData.Email
            };
            var result = await _userManager.CreateAsync(userEntity, formData.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new UserResult { Succeeded = false, StatusCode = 500, Error = errors };
            }

            return new UserResult { Succeeded = true, StatusCode = 201 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new UserResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }
}