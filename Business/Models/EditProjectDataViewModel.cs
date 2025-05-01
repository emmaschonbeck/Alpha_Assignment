namespace Business.Models;

public class EditProjectDataViewModel
{
    public string Id { get; set; } = null!;
    public string ProjectName { get; set; } = null!;
    public string ClientName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }
    public int StatusId { get; set; }
}
