using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class AddProjectViewModel
{
    [Required(ErrorMessage = "Project name is required")]
    public string ProjectName { get; set; } = null!;
    [Required(ErrorMessage = "Client name is required")]
    public string ClientName { get; set; } = null!;
    public string? Description { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Budget must be a positive number")]
    public decimal? Budget { get; set; }
}
