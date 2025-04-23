using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Models;

public class AddProjectViewModel
{
    public IEnumerable<SelectListItem> Clients { get; set; } = [];
    public IEnumerable<SelectListItem> Members { get; set; } = [];
}
