using Data.Repositories;

namespace Business.Models;

public class UserService(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<UserResult> GetUsersAsync()
    {
        var result = await _userRepository.GetAllAsync();
        return result.Succeeded
            ? new UserResult { Succeeded = true, StatusCode = result.StatusCode, Result = result.Result }
            : new UserResult { Succeeded = false, Error = "No statuses found." };
    }
}