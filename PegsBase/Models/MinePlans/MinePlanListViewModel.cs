using Microsoft.AspNetCore.Mvc.Rendering;
using PegsBase.Models.Enums;
using PegsBase.Models.MinePlans;

public class MinePlanListViewModel
{
    public List<MinePlan> Plans { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    // Filters
    public string? FilterSearch { get; set; }
    public string? FilterLevel { get; set; }
    public int? FilterPlanTypeId { get; set; }
    public IEnumerable<SelectListItem>? PlanTypeOptions { get; set; }
    public bool? FilterIsSigned { get; set; }
    public bool? FilterIsSuperseded { get; set; }
}
