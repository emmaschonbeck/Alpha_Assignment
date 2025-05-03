using Azure.Core;
using Business.Models;
using Data.Entities;
using Data.Models;
using Data.Repositories;

namespace Business.Services;

public interface IProjectService
{
    Task<ProjectResult> CreateProjectAsync(AddProjectFormData formData);
    Task<ProjectResult<object>> GetProjectAsync(string id);
    Task<ProjectResult<IEnumerable<ProjectEntity>>> GetProjectsAsync();
    Task<ProjectResult<IEnumerable<ProjectEntity>>> GetProjectsByUserAsync(string userId);
    Task<ProjectResult> UpdateProjectAsync(string id, EditProjectDataViewModel formData);
    Task<ProjectResult> DeleteProjectAsync(string id);
}

public class ProjectService(IProjectRepository projectrepository, IStatusService statusService) : IProjectService
{
    private readonly IProjectRepository _projectrepository = projectrepository;
    private readonly IStatusService _statusService = statusService;

    public async Task<ProjectResult> CreateProjectAsync(AddProjectFormData formData)
    {
        if (formData == null)
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

        var statusResult = await _statusService.GetStatusByIdAsync(formData.StatusId);
        if (!statusResult.Succeeded || statusResult.Result == null)
        {
            return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Invalid status ID." };
        }

        var projectEntity = new ProjectEntity
        {
            Id = Guid.NewGuid().ToString(),
            ProjectName = formData.ProjectName,
            ClientName = formData.ClientName,
            Description = formData.Description,
            StartDate = formData.StartDate,
            EndDate = formData.EndDate,
            Budget = formData.Budget,
            Created = DateTime.Now,
            UserId = formData.UserId,
            StatusId = formData.StatusId
        };

        var result = await _projectrepository.AddAsync(projectEntity);

        return result.Succeeded
            ? new ProjectResult { Succeeded = true, StatusCode = 201 }
            : new ProjectResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }

    public async Task<ProjectResult<IEnumerable<ProjectEntity>>> GetProjectsAsync()
    {
        var response = await _projectrepository.GetAllAsync
            (
                orderByDescending: true,
                sortBy: s => s.Created,
                where: null,
                include => include.User,
                include => include.Status
            );

        return new ProjectResult<IEnumerable<ProjectEntity>> { Succeeded = true, StatusCode = 200, Result = response.Result };
    }

    public async Task<ProjectResult<object>> GetProjectAsync(string id)
    {
        var response = await _projectrepository.GetAsync
            (
                where: x => x.Id == id,
                include => include.User,
                include => include.Status
            );

        return response.Succeeded
            ? new ProjectResult<object> { Succeeded = true, StatusCode = 200, Result = response.Result }
            : new ProjectResult<object> { Succeeded = false, StatusCode = 404, Error = $"Project '{id}' was not found" };
    }

    public async Task<ProjectResult<IEnumerable<ProjectEntity>>> GetProjectsByUserAsync(string userId)
    {
        var response = await _projectrepository.GetAllAsync
        (
            orderByDescending: true,
            sortBy: s => s.Created,
            where: x => x.UserId == userId,
            include => include.User,
            include => include.Status
        );

        return new ProjectResult<IEnumerable<ProjectEntity>>
        {
            Succeeded = true,
            StatusCode = 200,
            Result = response.Result
        };
    }

    public async Task<ProjectResult> UpdateProjectAsync(string id, EditProjectDataViewModel formData)
    {
        var response = await _projectrepository.GetAsync(x => x.Id == id);

        if (!response.Succeeded || response.Result == null)
            return new ProjectResult { Succeeded = false, StatusCode = 404, Error = $"Project '{id}' not found" };

        var project = response.Result;

        project.ProjectName = formData.ProjectName;
        project.ClientName = formData.ClientName;
        project.Description = formData.Description;
        project.StartDate = formData.StartDate;
        project.EndDate = formData.EndDate;
        project.Budget = formData.Budget;
        project.StatusId = formData.StatusId;

        var updateResult = await _projectrepository.UpdateAsync(project);

        return updateResult.Succeeded
            ? new ProjectResult { Succeeded = true, StatusCode = 200 }
            : new ProjectResult { Succeeded = false, StatusCode = updateResult.StatusCode, Error = updateResult.Error };
    }

    public async Task<ProjectResult> DeleteProjectAsync(string id)
    {
        var response = await _projectrepository.GetAsync(x => x.Id == id);

        if (!response.Succeeded || response.Result == null)
            return new ProjectResult { Succeeded = false, StatusCode = 404, Error = $"Project '{id}' not found" };

        var deleteResult = await _projectrepository.DeleteAsync(response.Result);

        return deleteResult.Succeeded
            ? new ProjectResult { Succeeded = true, StatusCode = 200 }
            : new ProjectResult { Succeeded = false, StatusCode = deleteResult.StatusCode, Error = deleteResult.Error };
    }
}

