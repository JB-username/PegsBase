using Microsoft.EntityFrameworkCore;
using PegsBase.Data;
using PegsBase.Models;
using PegsBase.Services.Parsing.Interfaces;
using System.Globalization;
using PegsBase.Models.ViewModels;

namespace PegsBase.Services.Parsing
{
    public class RawSurveyDataDatFileParser : IRawSurveyDataDatFileParser
    {
        
        public async Task<List<PegCalcViewModel>> ParseRawSurveyFileAsync(IFormFile file)
        {
            var rawDataList = new List<PegCalcViewModel>();
            string surveyor = string.Empty;
            DateOnly surveyDate = default;
            string place = string.Empty;
            string setupPeg = string.Empty;
            string backsightPeg = string.Empty;
            decimal instrumentHeight = 0;
            bool pegFailed = false;

            PegCalcViewModel rawDataViewModel = null;

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    var values = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    switch (values[0])
                    {
                        case "SURVEYOR":
                            surveyor = values[1];
                            break;

                        case "DATE":
                            DateOnly.TryParseExact(values[1], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out surveyDate);
                            break;

                        case "PLACE":
                            var placeParts = values.Skip(1).ToList();
                            pegFailed = placeParts.Contains("failed", StringComparer.OrdinalIgnoreCase);
                            place = string.Join(" ", placeParts.Where(p => !p.Equals("failed", StringComparison.OrdinalIgnoreCase)));
                            break;

                        case "SETUP":
                            setupPeg = values[1];
                            decimal.TryParse(values[2], NumberStyles.Any, CultureInfo.InvariantCulture, out instrumentHeight);
                            break;

                        case "BACKSIGHT":
                            backsightPeg = values[1];
                            break;

                        case "DIR1":
                        case "DIR2":
                        case "TRN1":
                        case "TRN2":
                            if (rawDataViewModel == null)
                            {
                                rawDataViewModel = new PegCalcViewModel
                                {
                                    Surveyor = surveyor,
                                    SurveyDate = surveyDate,
                                    Locality = place,
                                    StationPeg = setupPeg,
                                    BackSightPeg = backsightPeg,
                                    InstrumentHeight = instrumentHeight,
                                    PegFailed = pegFailed
                                };
                            }

                            string pegName = values[1];
                            bool isBacksight = pegName == backsightPeg;
                            bool isDir = values[0].StartsWith("DIR");
                            bool isTrn = values[0].StartsWith("TRN");
                            bool isSecondArc = values[0].EndsWith("2");

                            // ✅ Correctly map FrontSightPeg
                            if (!isBacksight && string.IsNullOrEmpty(rawDataViewModel.ForeSightPeg))
                            {
                                rawDataViewModel.ForeSightPeg = pegName;
                            }

                            
                            decimal.TryParse(values[2], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal horizontalAngle);
                            decimal.TryParse(values[3], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal verticalAngle);
                            decimal.TryParse(values[4], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal slopeDistance);
                            decimal.TryParse(values[5], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal targetHeight);

                            if (isDir)
                            {
                                if (isBacksight)
                                {
                                    if (isSecondArc)
                                    {
                                        rawDataViewModel.HAngleDirectArc2Backsight = horizontalAngle;
                                        rawDataViewModel.VAngleDirectArc2Backsight = verticalAngle;
                                    }
                                    else
                                    {
                                        rawDataViewModel.HAngleDirectArc1Backsight = horizontalAngle;
                                        rawDataViewModel.VAngleDirectArc1Backsight = verticalAngle;
                                    }

                                    
                                    if (rawDataViewModel.SlopeDistanceBacksight == 0)
                                        rawDataViewModel.SlopeDistanceBacksight = slopeDistance;

                                    if (rawDataViewModel.TargetHeightBacksight == 0)
                                        rawDataViewModel.TargetHeightBacksight = targetHeight;                                   
                                    
                                }
                                else
                                {
                                    if (isSecondArc)
                                    {
                                        rawDataViewModel.HAngleDirectArc2Foresight = horizontalAngle;
                                        rawDataViewModel.VAngleDirectArc2Foresight = verticalAngle;
                                    }
                                    else
                                    {
                                        rawDataViewModel.HAngleDirectArc1Foresight = horizontalAngle;
                                        rawDataViewModel.VAngleDirectArc1Foresight = verticalAngle;
                                    }

                                    if (rawDataViewModel.SlopeDistanceForesight == 0)
                                        rawDataViewModel.SlopeDistanceForesight = slopeDistance;

                                    if (rawDataViewModel.TargetHeightForesight == 0)
                                        rawDataViewModel.TargetHeightForesight = targetHeight;
                                }
                            }
                            else if (isTrn)
                            {
                                if (isBacksight)
                                {
                                    if (isSecondArc)
                                    {
                                        rawDataViewModel.HAngleTransitArc2Backsight = horizontalAngle;
                                        rawDataViewModel.VAngleTransitArc2Backsight = verticalAngle;
                                    }
                                    else
                                    {
                                        rawDataViewModel.HAngleTransitArc1Backsight = horizontalAngle;
                                        rawDataViewModel.VAngleTransitArc1Backsight = verticalAngle;
                                    }
                                }
                                else
                                {
                                    if (isSecondArc)
                                    {
                                        rawDataViewModel.HAngleTransitArc2Foresight = horizontalAngle;
                                        rawDataViewModel.VAngleTransitArc2Foresight = verticalAngle;
                                    }
                                    else
                                    {
                                        rawDataViewModel.HAngleTransitArc1Foresight = horizontalAngle;
                                        rawDataViewModel.VAngleTransitArc1Foresight = verticalAngle;
                                    }
                                }
                            }
                            break;
                    }
                }

                if (rawDataViewModel != null)
                {
                    rawDataList.Add(rawDataViewModel);
                }
            }

            return rawDataList;
        }
    }
}
