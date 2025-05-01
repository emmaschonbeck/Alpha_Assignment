using Business.Models;

namespace WebApp.Models;

public class ProjectsViewModel
{
    public List<ProjectViewModel> Projects { get; set; } = [];
    public Dictionary<string, int> StatusCounts { get; set; } = new();
    public string ActiveStatus { get; set; } = "all";
    public AddProjectFormData AddProjectFormData { get; set; } = new();
    public EditProjectDataViewModel EditProjectFormData { get; set; } = new();


}

