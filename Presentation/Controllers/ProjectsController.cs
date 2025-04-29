using Business.Models;
using Business.Services;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        if (!ModelState.IsValid)
            return View(model);

        var formData = new AddProjectFormData
        {
            ProjectName = model.ProjectName,
            ClientName = model.ClientName,
            Description = model.Description,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Budget = model.Budget,
            ClientId = "some-client-id",
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
            StatusId = 1
        };

        var result = await _projectService.CreateProjectAsync(formData);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Kunde inte skapa projekt.");
            return View(model);
        }

        return RedirectToAction("Index", "Projects");
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
    public async Task<IActionResult> Delete([FromBody] dynamic body)
    {
        string id = body.id;
        if (string.IsNullOrEmpty(id))
            return BadRequest("Ogiltigt ID");

        var result = await _projectService.DeleteProjectAsync(id);

        if (!result.Succeeded)
            return BadRequest(result.Error);

        return Ok();
    }

}
