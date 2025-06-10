using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.Settings;
using CavalryCivil3DPlugin._ACADLibrary.Geometry;
using CavalryCivil3DPlugin._C3DLibrary.Elements;
using CavalryCivil3DPlugin.C3DLibrary.Selection;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.ViewModels
{
    public class LowerPipeViewModel
    {
        public readonly Document AutocadDocument;
        public readonly CivilDocument Civil3DDocument;
        public readonly Database _Database;
        public readonly Editor _Editor;

        ObjectId upperPipeID;
        ObjectId lowerPipeID;

        public LowerPipeViewModel()
        {
            AutocadDocument = Application.DocumentManager.MdiActiveDocument;
            Civil3DDocument = CivilApplication.ActiveDocument;
            //AutocadDocument = acad;
            //Civil3DDocument = cdoc;
            //AutocadDocument = _autocadDocument;
            //Civil3DDocument = _civilDocument;
            _Database = AutocadDocument.Database;
            _Editor = AutocadDocument.Editor;

            //upperPipeID = upperPipeId;
            //lowerPipeID = lowerPipeId;

            LowerPipeExecuteMain();
        }


        public void LowerPipeExecuteMain()
        {
            ObjectId pipeUpperId = C3DObjectSelection.PickPressurePipe(AutocadDocument, "Pressure Pipe to be on top");
            ObjectId pipeLoweredId = C3DObjectSelection.PickPressurePipe(AutocadDocument, "Pressure Pipe to be lowered");

            //ObjectId pipeUpperId = upperPipeID;
            //ObjectId pipeLoweredId = lowerPipeID;


            double pipeVerticalClearance = 0.5;
            double pipeLateralClearance = 1;
            double maxDeflection = 8;


            //ObjectId newProfileId = ObjectId.Null;

            //using (DocumentLock docLock = AutocadDocument.LockDocument())
            //{
            using (Transaction tr = _Database.TransactionManager.StartTransaction())
            {
                PressurePipe pressurePipeUpper = tr.GetObject(pipeUpperId, OpenMode.ForRead) as PressurePipe;
                PressurePipe pressurePipeLower = tr.GetObject(pipeLoweredId, OpenMode.ForRead) as PressurePipe;

                Point3d pipeUpperp1 = pressurePipeUpper.StartPoint;
                Point3d pipeUpperp2 = pressurePipeUpper.EndPoint;
                Point3d pipeLowerp1 = pressurePipeLower.StartPoint;
                Point3d pipeLowerp2 = pressurePipeLower.EndPoint;

                (Point2d, Point2d) line1 = (new Point2d(pipeUpperp1.X, pipeUpperp1.Y), new Point2d(pipeUpperp2.X, pipeUpperp2.Y));
                (Point2d, Point2d) line2 = (new Point2d(pipeLowerp1.X, pipeLowerp1.Y), new Point2d(pipeLowerp2.X, pipeLowerp2.Y));

                Point2d intersectionPoint = Lines.GetIntersectionPoint(line1, line2);

                ObjectId pipeRunAlignmentIdUpper = _PressurePipe.GetAlignmentId(pipeUpperId, AutocadDocument);
                ObjectId pipeRunProfileIdUpper = _PressurePipe.GetProfileId(pipeUpperId, AutocadDocument);

                ObjectId pipeRunAlignmentIdLower = _PressurePipe.GetAlignmentId(pipeLoweredId, AutocadDocument);
                ObjectId pipeRunProfileIdLower = _PressurePipe.GetProfileId(pipeLoweredId, AutocadDocument);


                if (pipeRunAlignmentIdUpper != null && pipeRunAlignmentIdLower != null)
                {
                    Alignment pipeRunAlignmentUpper = tr.GetObject(pipeRunAlignmentIdUpper, OpenMode.ForRead) as Alignment;
                    Alignment pipeRunAlignmentLower = tr.GetObject(pipeRunAlignmentIdLower, OpenMode.ForRead) as Alignment;
                    Profile pipeRunProfileUpper = tr.GetObject(pipeRunProfileIdUpper, OpenMode.ForRead) as Profile;
                    Profile pipeRunProfileLower = tr.GetObject(pipeRunProfileIdLower, OpenMode.ForRead) as Profile;

                    //Computing Data at intersection of 2 Pipes
                    double stationUpperPipe = 0;
                    double offset1 = 0;
                    double elevationUpperPipe;

                    double stationLowerPipe = 0;
                    double offset2 = 0;
                    double elevationLowerPipe;

                    pipeRunAlignmentUpper.StationOffset(intersectionPoint.X, intersectionPoint.Y, ref stationUpperPipe, ref offset1);
                    pipeRunAlignmentLower.StationOffset(intersectionPoint.X, intersectionPoint.Y, ref stationLowerPipe, ref offset2);

                    elevationUpperPipe = pipeRunProfileUpper.ElevationAt(stationUpperPipe);
                    elevationLowerPipe = pipeRunProfileLower.ElevationAt(stationLowerPipe);


                    // Constant Parameters
                    double pipeUpperSize = pressurePipeUpper.OuterDiameter;
                    double pipeLowerSize = pressurePipeLower.OuterDiameter;

                    double loweredElevation = elevationUpperPipe - pipeUpperSize - pipeVerticalClearance;
                    double loweredStation1 = stationLowerPipe - pipeLateralClearance;
                    double loweredStation2 = stationLowerPipe + pipeLateralClearance;

                    double initialVDiff = pipeUpperSize + pipeVerticalClearance;


                    // Start Transition
                    double startDeflection = maxDeflection;
                    double upperStartAngle;
                    double startStation1;
                    double startElevation1;
                    double finalTransitionStation1;
                    double finalTransitionElevation1;
                    double startX1 = (initialVDiff / Tangent(startDeflection)) * -1;
                    double startX2;


                    while (true)
                    {

                        startX2 = startX1 - 0.3;
                        double startY1 = pipeRunProfileLower.ElevationAt(loweredStation1 - Math.Abs(startX1)) - loweredElevation;
                        double startY2 = pipeRunProfileLower.ElevationAt(loweredStation1 - Math.Abs(startX2)) - loweredElevation;

                        var l1 = (new Point2d(startX1, startY1), new Point2d(startX2, startY2));
                        var l2 = (new Point2d(0, 0), new Point2d(-1, Tangent(startDeflection)));

                        Point2d intersection = Lines.GetIntersectionPoint(l1, l2);

                        finalTransitionStation1 = loweredStation1 - Math.Abs(intersection.X);
                        finalTransitionElevation1 = pipeRunProfileLower.ElevationAt(finalTransitionStation1);

                        startStation1 = finalTransitionStation1 - 0.3;
                        startElevation1 = pipeRunProfileLower.ElevationAt(startStation1);


                        double actualStartDeflection = ArcTangent((finalTransitionElevation1 - loweredElevation) / (finalTransitionStation1 - loweredStation1));
                        if (actualStartDeflection > startDeflection)
                        {
                            startX1 = loweredStation1 - finalTransitionStation1;
                            continue;
                        }

                        upperStartAngle = ArcTangent((finalTransitionElevation1 - startElevation1) / (finalTransitionStation1 - startStation1));
                        if (upperStartAngle > 0)
                        {
                            if ((upperStartAngle + startDeflection) > maxDeflection)
                            {
                                startDeflection = maxDeflection - upperStartAngle;
                                startX1 = (initialVDiff / Tangent(startDeflection)) * -1;
                                continue;
                            }
                        }

                        break;
                    }


                    // End Transition
                    double upperEndDeflection = maxDeflection;
                    double upperEndAngle;
                    double endStation;
                    double endElevation;
                    double finalTransitionStation2;
                    double finalTransitionElevation2;
                    double endX1 = initialVDiff / Tangent(upperEndDeflection);

                    while (true)
                    {
                        double endX2 = endX1 + 0.3;

                        double endY1 = pipeRunProfileLower.ElevationAt(loweredStation2 + Math.Abs(endX1)) - loweredElevation;
                        double endY2 = pipeRunProfileLower.ElevationAt(loweredStation2 + Math.Abs(endX2)) - loweredElevation;

                        var endl1 = (new Point2d(endX1, endY1), new Point2d(endX2, endY2));
                        var endl2 = (new Point2d(0, 0), new Point2d(1, Tangent(upperEndDeflection)));

                        Point2d intersectionEnd = Lines.GetIntersectionPoint(endl1, endl2);

                        _Console.ShowConsole(intersectionEnd.ToString());

                        finalTransitionStation2 = loweredStation2 + Math.Abs(intersectionEnd.X);
                        finalTransitionElevation2 = pipeRunProfileLower.ElevationAt(finalTransitionStation2);
                        endStation = finalTransitionStation2 + 0.3;
                        endElevation = pipeRunProfileLower.ElevationAt(endStation);

                        double actualEndtDeflection = ArcTangent((finalTransitionElevation2 - loweredElevation) / (finalTransitionStation2 - loweredStation2));
                        if (actualEndtDeflection > upperEndDeflection)
                        {
                            endX1 = finalTransitionStation2 - loweredStation2;
                            continue;
                        }

                        upperEndAngle = ArcTangent((endElevation - finalTransitionElevation2) / (endStation - finalTransitionStation2));
                        if (upperEndAngle < 0)
                        {
                            if ((Math.Abs(upperEndAngle) + startDeflection) > maxDeflection)
                            {
                                upperEndDeflection = maxDeflection - upperEndAngle;
                                endX1 = initialVDiff / Tangent(upperEndDeflection);
                                continue;
                            }
                        }

                        break;
                    }


                    // Summary
                    var p1 = (startStation1, startElevation1);
                    var p2 = (finalTransitionStation1, finalTransitionElevation1);
                    var p3 = (loweredStation1, loweredElevation);
                    var p4 = (loweredStation2, loweredElevation);
                    var p5 = (finalTransitionStation2, finalTransitionElevation2);
                    var p6 = (endStation, endElevation);


                    // Creation of Profile
                    SettingsCmdCreateProfileLayout settingsProfile = Civil3DDocument.Settings.GetSettings<SettingsCmdCreateProfileLayout>();
                    ObjectId profileStyleId = settingsProfile.StyleSettings.ProfileStyleId.Value;
                    ObjectId labelSetId = settingsProfile.StyleSettings.ProfileLabelSetId.Value;
                    ObjectId layerId = pipeRunProfileLower.LayerId;
                    string profileLowerName = pipeRunProfileLower.Name;
                    int overrideIndex = 1;
                    string profileOverrideName = $"{profileLowerName} [Override - {overrideIndex}]";

                    //_Console.ShowConsole(
                    //pipeRunAlignmentIdLower.ToString() +
                    //"\n" +
                    //layerId.ToString() +
                    //"\n" +
                    //profileStyleId.ToString() +
                    //"\n" +
                    //labelSetId.ToString()
                    //);

                    ObjectId newProfileId = Profile.CreateByLayout(profileOverrideName, pipeRunAlignmentIdLower, layerId, profileStyleId, labelSetId);

                    Profile newProfile = tr.GetObject(newProfileId, OpenMode.ForWrite) as Profile;


                    ProfilePVICollection pVIs = newProfile.PVIs;
                    pVIs.AddPVI(p1.Item1, p1.Item2);
                    pVIs.AddPVI(p2.Item1, p2.Item2);
                    pVIs.AddPVI(p3.Item1, p3.Item2);
                    pVIs.AddPVI(p4.Item1, p4.Item2);
                    pVIs.AddPVI(p5.Item1, p5.Item2);
                    pVIs.AddPVI(p6.Item1, p6.Item2);

                    tr.Commit();
                }
            }
            //}

            try
            {
                //_PressurePipe.SetProfileToPipeRun(AutocadDocument, pipeLoweredId, newProfileId);
            }
            catch (Exception ex) { _Console.ShowConsole(ex.ToString()); }
        }


        public void LowerPipeExecuteMains()
        {
            ObjectId pipeUpperId = C3DObjectSelection.PickPressurePipe(AutocadDocument, "Pressure Pipe to be on top");
            ObjectId pipeLoweredId = C3DObjectSelection.PickPressurePipe(AutocadDocument, "Pressure Pipe to be lowered");

            //ObjectId pipeUpperId = upperPipeID;
            //ObjectId pipeLoweredId = lowerPipeID;


            double pipeVerticalClearance = 0.5;
            double pipeLateralClearance = 1;
            double maxDeflection = 8;
            

            //ObjectId newProfileId = ObjectId.Null;

            //using (DocumentLock docLock = AutocadDocument.LockDocument())
            //{
                using (Transaction tr = _Database.TransactionManager.StartTransaction())
                {
                    PressurePipe pressurePipeUpper = tr.GetObject(pipeUpperId, OpenMode.ForRead) as PressurePipe;
                    PressurePipe pressurePipeLower = tr.GetObject(pipeLoweredId, OpenMode.ForRead) as PressurePipe;

                    Point3d pipeUpperp1 = pressurePipeUpper.StartPoint;
                    Point3d pipeUpperp2 = pressurePipeUpper.EndPoint;
                    Point3d pipeLowerp1 = pressurePipeLower.StartPoint;
                    Point3d pipeLowerp2 = pressurePipeLower.EndPoint;

                    (Point2d, Point2d) line1 = (new Point2d(pipeUpperp1.X, pipeUpperp1.Y), new Point2d(pipeUpperp2.X, pipeUpperp2.Y));
                    (Point2d, Point2d) line2 = (new Point2d(pipeLowerp1.X, pipeLowerp1.Y), new Point2d(pipeLowerp2.X, pipeLowerp2.Y));

                    Point2d intersectionPoint = Lines.GetIntersectionPoint(line1, line2);

                    ObjectId pipeRunAlignmentIdUpper = _PressurePipe.GetAlignmentId(pipeUpperId, AutocadDocument);
                    ObjectId pipeRunProfileIdUpper = _PressurePipe.GetProfileId(pipeUpperId, AutocadDocument);

                    ObjectId pipeRunAlignmentIdLower = _PressurePipe.GetAlignmentId(pipeLoweredId, AutocadDocument);
                    ObjectId pipeRunProfileIdLower = _PressurePipe.GetProfileId(pipeLoweredId, AutocadDocument);


                    if (pipeRunAlignmentIdUpper != null && pipeRunAlignmentIdLower != null)
                    {
                        Alignment pipeRunAlignmentUpper = tr.GetObject(pipeRunAlignmentIdUpper, OpenMode.ForRead) as Alignment;
                        Alignment pipeRunAlignmentLower = tr.GetObject(pipeRunAlignmentIdLower, OpenMode.ForRead) as Alignment;
                        Profile pipeRunProfileUpper = tr.GetObject(pipeRunProfileIdUpper, OpenMode.ForRead) as Profile;
                        Profile pipeRunProfileLower = tr.GetObject(pipeRunProfileIdLower, OpenMode.ForRead) as Profile;

                        //Computing Data at intersection of 2 Pipes
                        double stationUpperPipe = 0;
                        double offset1 = 0;
                        double elevationUpperPipe;

                        double stationLowerPipe = 0;
                        double offset2 = 0;
                        double elevationLowerPipe;

                        pipeRunAlignmentUpper.StationOffset(intersectionPoint.X, intersectionPoint.Y, ref stationUpperPipe, ref offset1);
                        pipeRunAlignmentLower.StationOffset(intersectionPoint.X, intersectionPoint.Y, ref stationLowerPipe, ref offset2);

                        elevationUpperPipe = pipeRunProfileUpper.ElevationAt(stationUpperPipe);
                        elevationLowerPipe = pipeRunProfileLower.ElevationAt(stationLowerPipe);


                        // Constant Parameters
                        double pipeUpperSize = pressurePipeUpper.OuterDiameter;
                        double pipeLowerSize = pressurePipeLower.OuterDiameter;

                        double loweredElevation = elevationUpperPipe - pipeUpperSize - pipeVerticalClearance;
                        double loweredStation1 = stationLowerPipe - pipeLateralClearance;
                        double loweredStation2 = stationLowerPipe + pipeLateralClearance;

                        double initialVDiff = pipeUpperSize + pipeVerticalClearance;


                        // Start Transition
                        double startDeflection = maxDeflection;
                        double upperStartAngle;
                        double startStation1;
                        double startElevation1;
                        double finalTransitionStation1;
                        double finalTransitionElevation1;
                        double startX1 = (initialVDiff / Tangent(startDeflection)) * -1;
                        double startX2;
                        

                        while (true)
                        {
                            
                            startX2 = startX1 - 0.3;
                            double startY1 = pipeRunProfileLower.ElevationAt(loweredStation1 - Math.Abs(startX1)) - loweredElevation;
                            double startY2 = pipeRunProfileLower.ElevationAt(loweredStation1 - Math.Abs(startX2)) - loweredElevation;

                            var l1 = (new Point2d(startX1, startY1), new Point2d(startX2, startY2));
                            var l2 = (new Point2d(0, 0), new Point2d(-1, Tangent(startDeflection)));

                            Point2d intersection = Lines.GetIntersectionPoint(l1, l2);

                            finalTransitionStation1 = loweredStation1 - Math.Abs(intersection.X);
                            finalTransitionElevation1 = pipeRunProfileLower.ElevationAt(finalTransitionStation1);

                            startStation1 = finalTransitionStation1 - 0.3;
                            startElevation1 = pipeRunProfileLower.ElevationAt(startStation1);


                            double actualStartDeflection = ArcTangent((finalTransitionElevation1 - loweredElevation) / (finalTransitionStation1 - loweredStation1));
                            if (actualStartDeflection > startDeflection)
                            {
                                startX1 = loweredStation1 - finalTransitionStation1;
                                continue;
                            }

                            upperStartAngle = ArcTangent((finalTransitionElevation1 - startElevation1) / (finalTransitionStation1 - startStation1));
                            if (upperStartAngle > 0)
                            {
                                if ((upperStartAngle + startDeflection) > maxDeflection)
                                {
                                    startDeflection = maxDeflection - upperStartAngle;
                                    startX1 = (initialVDiff / Tangent(startDeflection)) * -1;
                                    continue;
                                }
                            }

                            break;
                        }


                        // End Transition
                        double upperEndDeflection = maxDeflection;
                        double upperEndAngle;
                        double endStation;
                        double endElevation;
                        double finalTransitionStation2;
                        double finalTransitionElevation2;
                        double endX1 = initialVDiff / Tangent(upperEndDeflection);

                        while (true)
                        {
                            double endX2 = endX1 + 0.3;

                            double endY1 = pipeRunProfileLower.ElevationAt(loweredStation2 + Math.Abs(endX1)) - loweredElevation;
                            double endY2 = pipeRunProfileLower.ElevationAt(loweredStation2 + Math.Abs(endX2)) - loweredElevation;

                            var endl1 = (new Point2d(endX1, endY1), new Point2d(endX2, endY2));
                            var endl2 = (new Point2d(0, 0), new Point2d(1, Tangent(upperEndDeflection)));

                            Point2d intersectionEnd = Lines.GetIntersectionPoint(endl1, endl2);

                            _Console.ShowConsole(intersectionEnd.ToString());

                            finalTransitionStation2 = loweredStation2 + Math.Abs(intersectionEnd.X);
                            finalTransitionElevation2 = pipeRunProfileLower.ElevationAt(finalTransitionStation2);
                            endStation = finalTransitionStation2 + 0.3;
                            endElevation = pipeRunProfileLower.ElevationAt(endStation);

                            double actualEndtDeflection = ArcTangent((finalTransitionElevation2 - loweredElevation) / (finalTransitionStation2 - loweredStation2));
                            if (actualEndtDeflection > upperEndDeflection)
                            {
                                endX1 = finalTransitionStation2 - loweredStation2;
                                continue;
                            }

                            upperEndAngle = ArcTangent((endElevation - finalTransitionElevation2) / (endStation - finalTransitionStation2));
                            if (upperEndAngle < 0)
                            {
                                if ((Math.Abs(upperEndAngle) + startDeflection) > maxDeflection)
                                {
                                    upperEndDeflection = maxDeflection - upperEndAngle;
                                    endX1 = initialVDiff / Tangent(upperEndDeflection);
                                    continue;
                                }
                            }

                            break;
                        }
                                                                                      

                        // Summary
                        var p1 = (startStation1, startElevation1);
                        var p2 = (finalTransitionStation1, finalTransitionElevation1);
                        var p3 = (loweredStation1, loweredElevation);
                        var p4 = (loweredStation2, loweredElevation);
                        var p5 = (finalTransitionStation2, finalTransitionElevation2);
                        var p6 = (endStation, endElevation);


                        // Creation of Profile
                        SettingsCmdCreateProfileLayout settingsProfile = Civil3DDocument.Settings.GetSettings<SettingsCmdCreateProfileLayout>();
                        ObjectId profileStyleId = settingsProfile.StyleSettings.ProfileStyleId.Value;
                        ObjectId labelSetId = settingsProfile.StyleSettings.ProfileLabelSetId.Value;
                        ObjectId layerId = pipeRunProfileLower.LayerId;
                        string profileLowerName = pipeRunProfileLower.Name;
                        int overrideIndex = 1;
                        string profileOverrideName = $"{profileLowerName} [Override - {overrideIndex}]";

                        _Console.ShowConsole(
                        pipeRunAlignmentIdLower.ToString() +
                        "\n" +
                        layerId.ToString() +
                        "\n" +
                        profileStyleId.ToString() +
                        "\n" +
                        labelSetId.ToString()
                        );

                        ObjectId newProfileId = Profile.CreateByLayout(profileOverrideName, pipeRunAlignmentIdLower, layerId, profileStyleId, labelSetId);

                        Profile newProfile = tr.GetObject(newProfileId, OpenMode.ForWrite) as Profile;


                        ProfilePVICollection pVIs = newProfile.PVIs;
                        pVIs.AddPVI(p1.Item1, p1.Item2);
                        pVIs.AddPVI(p2.Item1, p2.Item2);
                        pVIs.AddPVI(p3.Item1, p3.Item2);
                        pVIs.AddPVI(p4.Item1, p4.Item2);
                        pVIs.AddPVI(p5.Item1, p5.Item2);
                        pVIs.AddPVI(p6.Item1, p6.Item2);

                        tr.Commit();
                    }
                }
            //}

            try
            {
                //_PressurePipe.SetProfileToPipeRun(AutocadDocument, pipeLoweredId, newProfileId);
            }
            catch (Exception ex) {_Console.ShowConsole(ex.ToString());} 
        }




        private double Tangent(double x)
        {
            return Math.Tan(x * (Math.PI / 180));
        }

        private double ArcTangent(double x)
        {
            return Math.Atan(x) * (180 /  Math.PI);
        }

    }
}
