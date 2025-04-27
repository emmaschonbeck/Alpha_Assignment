using Business.Models;
using Data.Repositories;

namespace Business.Services;

public interface IStatusService
{
    Task<StatusResult> GetStatusesAsync();
}

public class StatusService(IStatusRepository statusRepository) : IStatusService
{
    private readonly IStatusRepository _statusRepository = statusRepository;

    public async Task<StatusResult> GetStatusesAsync()
    {
        var result = await _statusRepository.GetAllAsync();
        return result.Succeeded
            ? new StatusResult { Succeeded = true, StatusCode = result.StatusCode, Result = result.Result }
            : new StatusResult { Succeeded = false, Error = "No statuses found." };
    }
}