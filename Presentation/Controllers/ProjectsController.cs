using Business.Models;
using Business.Services;
using Data.Entities;
using Data.Contexts;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using WebApp.Models;
using System.Threading.Tasks;

namespace WebApp.Controllers;

[Authorize]
public class ProjectsController(IProjectService projectService) : Controller
{
    private readonly IProjectService _projectService = projectService;

    [Route("admin/projects")]
    public async Task<IActionResult> Index(string? status)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _projectService.GetProjectsByUserAsync(userId);

        if (!result.Succeeded || result.Result == null)
            return View(new ProjectsViewModel());

        var allProjects = result.Result.ToList();

        var viewModel = new ProjectsViewModel
        {
            Projects = (status?.ToLower() switch
            {
                "started" => allProjects.Where(p => p.Status.StatusName == "Started"),
                "completed" => allProjects.Where(p => p.Status.StatusName == "Completed"),
                _ => allProjects
            })
         .Select(entity => new ProjectViewModel
         {
             Id = entity.Id,
             ProjectName = entity.ProjectName,
             ClientName = entity.ClientName,
             Description = entity.Description,
             StartDate = entity.StartDate,
             EndDate = entity.EndDate,
             Budget = entity.Budget,
             StatusId = entity.StatusId
         }).ToList(),

            StatusCounts = new Dictionary<string, int>
            {
                ["All"] = allProjects.Count,
                ["Started"] = allProjects.Count(p => p.Status.StatusName == "Started"),
                ["Completed"] = allProjects.Count(p => p.Status.StatusName == "Completed")
            },

            ActiveStatus = status ?? "all"
        };

        return View(viewModel);
    }

    [HttpGet]
    [Route("api/projects/{id}")]
    public async Task<IActionResult> GetProject(string id)
    {
        var result = await _projectService.GetProjectAsync(id);

        if (!result.Succeeded || result.Result == null)
            return NotFound("Project not found.");

        var entity = (ProjectEntity)result.Result;

        var viewModel = new ProjectViewModel
        {
            Id = entity.Id,
            ProjectName = entity.ProjectName,
            ClientName = entity.ClientName,
            Description = entity.Description,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Budget = entity.Budget,
            StatusId = entity.StatusId
        };

        return Json(viewModel);
    }



    [HttpGet]
    [Route("admin/projects/create")]
    public IActionResult Create()
    {
        return View();
    }

    /*
        Med hjälp av Chat GPT-4o - 
    */

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/projects/create")]
    public async Task<IActionResult> Create(AddProjectViewModel model)
    {

        var formData = new AddProjectFormData
        {
            ProjectName = model.ProjectName,
            ClientName = model.ClientName,
            Description = model.Description,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Budget = model.Budget,
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
            StatusId = model.StatusId
        };

        var result = await _projectService.CreateProjectAsync(formData);

        if (!result.Succeeded)
        {
            TempData["Error"] = result.Error ?? "Could not create.";
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/projects/edit/{id}")]
    public async Task<IActionResult> Edit([FromRoute] string id, [FromBody] EditProjectDataViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid data");

        var result = await _projectService.UpdateProjectAsync(id, model);

        if (!result.Succeeded)
            return BadRequest(result.Error);

        return Ok();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/projects/delete")]
    public async Task<IActionResult> Delete([FromBody] JsonElement body)
    {
        if (!body.TryGetProperty("id", out var idProperty))
            return BadRequest("Invalid ID");

        string id = idProperty.GetString();

        if (string.IsNullOrEmpty(id))
            return BadRequest("Invalid ID");

        var result = await _projectService.DeleteProjectAsync(id);

        if (!result.Succeeded)
            return BadRequest(result.Error);

        return Ok();
    }
}
