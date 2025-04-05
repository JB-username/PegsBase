namespace PegsBase.Models.Constants
{
    public static class RoleGroups
    {
        public static readonly string[] SurveyManagers =
        {
            Roles.Master,
            Roles.MineSurveyor,
            Roles.SurveyAnalyst
        };

        public static readonly string[] SurveyDepartment =
        {
            Roles.Master,
            Roles.MineSurveyor,
            Roles.SurveyAnalyst,
            Roles.Surveyor
        };

        public static readonly string[] CanUploadSignedNotes =
        {
            Roles.Master,
            Roles.MineSurveyor,
            Roles.SurveyAnalyst,
            Roles.Surveyor,
            Roles.Mrm
        };

        public static readonly string[] CanUploadInProgressNotes =
        {
            Roles.Master,
            Roles.MineSurveyor,
            Roles.SurveyAnalyst,
            Roles.Surveyor,
        };

        public static readonly string[] CanViewAllNotes =
        {
            Roles.Master,
            Roles.MineSurveyor,
            Roles.SurveyAnalyst,
            Roles.Surveyor,
            Roles.Mrm
        };
        

        public static readonly string[] CanDownloadSignedNotes =
        {
            Roles.Master,
            Roles.MineSurveyor,
            Roles.SurveyAnalyst,
            Roles.Surveyor,
            Roles.Mrm,
            Roles.MineManager,
            Roles.MineOverseer
        };

        public static readonly string[] TestRole =
        {
            Roles.Viewer
        };
    }
}
