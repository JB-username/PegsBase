using System.ComponentModel.DataAnnotations;

namespace PegsBase.Models.Enums
{
    public enum MinePlanType
    {
        [Display(Name = "Index Key Plan")]
        IndexKeyPlan,
        SurfacePlan,
        SurfaceContourPlan,
        MineVentAndRescuePlan,
        MinePlan,
        RehabilitationPlan,
        ResidueDeposit,
        Geological,
        Workings,
        VerticalSection,
        LevelPlan,
        GeneralPlan,
        CrossSections,
        MOPlan
    }
}
