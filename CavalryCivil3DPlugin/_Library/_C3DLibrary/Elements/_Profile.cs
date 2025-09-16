using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using Autodesk.Civil.Settings;
using CavalryCivil3DPlugin.C3DLibrary.Selection;
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Models;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin._C3DLibrary.Elements
{
    public class _Profile
    {

        public static ObjectId CreateProfile(Document _autocadDocument, CivilDocument _civilDocument, ObjectId _referenceAlignmentId, ObjectId _referenceProfileId,List<(double, double)> _stationElevation)
        {
            
            using (Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
            {

                Profile mainProfile = tr.GetObject(_referenceProfileId, OpenMode.ForRead) as Profile;
                SettingsCmdCreateProfileLayout settingsProfile = _civilDocument.Settings.GetSettings<SettingsCmdCreateProfileLayout>();
                
                ObjectId profileStyleId = settingsProfile.StyleSettings.ProfileStyleId.Value;
                ObjectId labelSetId = settingsProfile.StyleSettings.ProfileLabelSetId.Value;
                ObjectId layerId = mainProfile.LayerId;

                ObjectId newProfileId = ObjectId.Null;
                int n = 1;

                while (true)
                {
                    string newProfileName = $"{mainProfile.Name} [Override - {n}]";

                    try
                    {
                        newProfileId = Profile.CreateByLayout(newProfileName, _referenceAlignmentId, layerId, profileStyleId, labelSetId);
                        break;
                    }

                    catch (Exception ex)
                    {
                        n++;
                    }
                }

                Profile newProfile = tr.GetObject(newProfileId, OpenMode.ForWrite) as Profile;

                ProfilePVICollection pVIs = newProfile.PVIs;

                int index = 0;
                foreach (var points in _stationElevation)
                {
                    pVIs.AddPVI(points.Item1, points.Item2);
                    index++;
                }

                tr.Commit();
                return newProfileId;
            }
        }


        public static ObjectId ReCreateProfile(
            Document _autocadDocument, 
            CivilDocument _civilDocument, 
            ObjectId _referenceAlignmentId, 
            ObjectId _referenceProfileId,
            List<(double, double)> _stationElevation,
            bool _startOnly = false,
            bool _endOnly = false)
        {

            using (Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
            {

                Profile mainProfile = tr.GetObject(_referenceProfileId, OpenMode.ForRead) as Profile;
                SettingsCmdCreateProfileLayout settingsProfile = _civilDocument.Settings.GetSettings<SettingsCmdCreateProfileLayout>();

                ObjectId profileStyleId = settingsProfile.StyleSettings.ProfileStyleId.Value;
                ObjectId labelSetId = settingsProfile.StyleSettings.ProfileLabelSetId.Value;
                ObjectId layerId = mainProfile.LayerId;

                ObjectId newProfileId = ObjectId.Null;
                int n = 1;

                while (true)
                {
                    string newProfileName = $"{mainProfile.Name} [Override - {n}]";

                    try
                    {
                        newProfileId = Profile.CreateByLayout(newProfileName, _referenceAlignmentId, layerId, profileStyleId, labelSetId);
                        break;
                    }

                    catch (Exception ex)
                    {
                        n++;
                    }
                }

                Profile newProfile = tr.GetObject(newProfileId, OpenMode.ForWrite) as Profile;

                ProfilePVICollection mainProfilePVIs = mainProfile.PVIs;
                ProfilePVICollection newProfilePVIs = newProfile.PVIs;
                double startStation = _stationElevation.First().Item1;
                double endStation = _stationElevation.Last().Item1;

                foreach (ProfilePVI pvi in mainProfilePVIs)
                {
                    if (pvi.Station > startStation && pvi.Station < endStation)
                    {
                        continue;
                    }

                    newProfilePVIs.AddPVI(pvi.Station, pvi.Elevation);
                }


                int index = 0;

                if (_startOnly)
                {
                    foreach (var points in _stationElevation)
                    {
                        if (index == 0)
                        {
                            newProfilePVIs[0].Station = points.Item1;
                            newProfilePVIs[0].Elevation = points.Item2;
                            index++;
                            continue;
                        }

                        newProfilePVIs.AddPVI(points.Item1, points.Item2);
                    }
                }


                else if (_endOnly)
                {
                    foreach (var points in _stationElevation)
                    {
                        if (points == _stationElevation.Last())
                        {
                            newProfilePVIs.Last().Station = points.Item1;
                            newProfilePVIs.Last().Elevation = points.Item2;
                            break;
                        }

                        newProfilePVIs.AddPVI(points.Item1, points.Item2);
                    }
                }


                else
                {
                    foreach (var points in _stationElevation)
                    {
                        newProfilePVIs.AddPVI(points.Item1, points.Item2);
                    }
                }
                
                tr.Commit();
                return newProfileId;
            }
        }
    }
}
