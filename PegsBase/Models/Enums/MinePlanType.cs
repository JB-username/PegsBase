using System.ComponentModel.DataAnnotations;

namespace PegsBase.Models.Enums
{
    public enum MinePlanType
    {
        [Display(Name = "Index Key Plan")]
        IndexKeyPlan,
        [Display(Name = "Surface Plan")]
        SurfacePlan,
        [Display(Name = "Surface Contour Plan")]
        SurfaceContourPlan,
        [Display(Name = "Mine Vent And Rescue Plan")]
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
