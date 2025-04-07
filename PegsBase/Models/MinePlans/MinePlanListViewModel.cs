using PegsBase.Models.MinePlans;
using PegsBase.Models.Enums;

public class MinePlanListViewModel
{
    public List<MinePlan> Plans { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    // Filters
    public string? FilterSearch { get; set; }
    public string? FilterLevel { get; set; }
    public MinePlanType? FilterType { get; set; }
    public bool? FilterIsSigned { get; set; }
    public bool? FilterIsSuperseded { get; set; }
}
