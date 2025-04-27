using Business.Models;
using Data.Repositories;

namespace Business.Services;

public interface IClientService
{
    Task<ClientResult> GetClientsAsync();
}

public class ClientService(IClientRepository clientRepository) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;

    public async Task<ClientResult> GetClientsAsync()
    {
        var result = await _clientRepository.GetAllAsync();
        return result.Succeeded
            ? new ClientResult { Succeeded = true, StatusCode = result.StatusCode, Result = result.Result }
            : new ClientResult { Succeeded = false, Error = "No statuses found." };
    }
}
