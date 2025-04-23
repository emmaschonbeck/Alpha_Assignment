using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class ProjectsController : Controller
{
    [Route("admin/projects")]
    public IActionResult Index()
    {
        var viewModel = new ProjectsViewModel()
        {
            Projects = SetProjects()
        };

        return View(viewModel);
    }


    private IEnumerable<ProjectViewModel> SetProjects()
    {
        var projects = new List<ProjectViewModel>();

        projects.Add(new ProjectViewModel
        {
            Id = Guid.NewGuid().ToString(),
            ProjectName = "Website Redesign",
            ClientName = "GitLab Inc",
            Description = "<p>It is necessary to develop a website redesign in a corporate style.</p>"
        });

        return projects;
    }
}
