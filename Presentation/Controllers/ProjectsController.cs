using Business.Models;
using Business.Services;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using WebApp.Models;

namespace WebApp.Controllers;

[Authorize]
public class ProjectsController(IProjectService projectService) : Controller
{
    private readonly IProjectService _projectService = projectService;

    [Route("admin/projects")]
    public async Task<IActionResult> Index()
    {
        var result = await _projectService.GetProjectsAsync();

        if (!result.Succeeded || result.Result == null)
            return View(new ProjectsViewModel());

        var viewModel = new ProjectsViewModel
        {
            Projects = result.Result.Select(project =>
            {
                var entity = (ProjectEntity)project;
                return new ProjectViewModel
                {
                    Id = entity.Id,
                    ProjectName = entity.ProjectName,
                    ClientName = entity.ClientName,
                    Description = entity.Description
                };
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpGet]
    [Route("api/projects/{id}")]
    public async Task<IActionResult> GetProject(string id)
    {
        var result = await _projectService.GetProjectAsync(id);

        if (!result.Succeeded || result.Result == null)
            return NotFound("Projektet hittades inte.");

        var entity = (ProjectEntity)result.Result;

        var viewModel = new ProjectViewModel
        {
            Id = entity.Id,
            ProjectName = entity.ProjectName,
            ClientName = entity.ClientName,
            Description = entity.Description,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Budget = entity.Budget
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
            ClientId = "5E80CFDE-DD4F-4098-8AE4-4FF6F0D54828",
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
            StatusId = 1
        };

        var result = await _projectService.CreateProjectAsync(formData);

        if (!result.Succeeded)
        {
            TempData["Error"] = result.Error ?? "Kunde inte skapa projekt.";
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/projects/edit/{id}")]
    public async Task<IActionResult> Edit(string id, [FromBody] EditProjectDataViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest("Ogiltiga data");

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
            return BadRequest("Ogiltigt ID");

        string id = idProperty.GetString();

        if (string.IsNullOrEmpty(id))
            return BadRequest("Ogiltigt ID");

        var result = await _projectService.DeleteProjectAsync(id);

        if (!result.Succeeded)
            return BadRequest(result.Error);

        return Ok();
    }
}
