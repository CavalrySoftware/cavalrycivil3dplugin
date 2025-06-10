using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.Settings;
using CavalryCivil3DPlugin._ACADLibrary.Geometry;
using CavalryCivil3DPlugin._C3DLibrary.Elements;
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.ViewModels;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Models
{
    public class LoweringAnalysisModel
    {

        private LowerPipeMainViewModel _ViewModel;
        private Document _AutocadDocument;


        private Point2d _IntersectionPoint;
        public Point2d IntersectionPoint
        {
            get { return _IntersectionPoint; }
            set { _IntersectionPoint = value; }
        }


        private List<double> _ActualDeflections;
        public List<double> ActualDeflections { get { return _ActualDeflections; } }


        private double _VerticalClearance;
        public double VerticalClearance { get { return _VerticalClearance; } }

        private double _CrossingLength;
        private double _MaxDeflection;

        private double _LateralOffset;
        public double LateralOfsset { get { return _LateralOffset; } }

        private bool _ValidRequirements;
        public bool ValidRequirements
        {
            get { return _ValidRequirements; }
        }


        private double _LowerElevation;
        public double LowerElevation
        {
            get { return _LowerElevation; }
        }


        private List<(double, double)> _ProfileData;
        public List<(double, double)> ProfileData { get { return _ProfileData; } }


        public LoweringAnalysisModel(LowerPipeMainViewModel _viewModel)
        {
            _ViewModel = _viewModel;
            _AutocadDocument = _viewModel.AutocadDocument;
        }


        public void Analyze()
        {
            GetDataRequirements();
            if (_ValidRequirements)
            {
                _Analyze();
            }
        }


        private void GetDataRequirements()
        {
            if  (
                double.TryParse(_ViewModel.VerticalClearance, out double result1) &&
                double.TryParse(_ViewModel.CrossingLength, out double result2) &&
                double.TryParse(_ViewModel.MaxDeflection, out double result3)
                )
            {
                _VerticalClearance = result1;
                _CrossingLength = result2;
                _MaxDeflection = result3;
                _LateralOffset = _CrossingLength / 2; 
                _ValidRequirements = true;
            }

            else
            {
                _ValidRequirements = false;
            }
        }


        private void _Analyze()
        {
            
            _IntersectionPoint = _PressurePipe.GetIntersectionPoint(_AutocadDocument, _ViewModel.UpperPipe.ObjectId_, _ViewModel.LowerPipe.ObjectId_);

            using (Transaction tr = _AutocadDocument.Database.TransactionManager.StartTransaction())
            {
                ObjectId _PipeRunProfileUpperId = _PressurePipe.GetProfileId(_ViewModel.UpperPipe.ObjectId_, _AutocadDocument);
                ObjectId _PipeRunAlignmentUpperId = _PressurePipe.GetAlignmentId(_ViewModel.UpperPipe.ObjectId_, _AutocadDocument);
                ObjectId _PipeRunProfileLowerId = _PressurePipe.GetProfileId(_ViewModel.LowerPipe.ObjectId_, _AutocadDocument);
                ObjectId _PipeRunAlignmentLowerId = _PressurePipe.GetAlignmentId(_ViewModel.LowerPipe.ObjectId_, _AutocadDocument);

                Alignment pipeRunAlignmentUpper = tr.GetObject(_PipeRunAlignmentUpperId, OpenMode.ForRead) as Alignment;
                Alignment pipeRunAlignmentLower = tr.GetObject(_PipeRunAlignmentLowerId, OpenMode.ForRead) as Alignment;
                Profile pipeRunProfileUpper = tr.GetObject(_PipeRunProfileUpperId, OpenMode.ForRead) as Profile;
                Profile pipeRunProfileLower = tr.GetObject(_PipeRunProfileLowerId, OpenMode.ForRead) as Profile;
                
                //Computing Data at intersection of 2 Pipes
                double stationUpperPipe = 0;
                double offset1 = 0;
                double elevationUpperPipe;

                double stationLowerPipe = 0;
                double offset2 = 0;
                double elevationLowerPipe;

                pipeRunAlignmentUpper.StationOffset(_IntersectionPoint.X, _IntersectionPoint.Y, ref stationUpperPipe, ref offset1);
                pipeRunAlignmentLower.StationOffset(_IntersectionPoint.X, _IntersectionPoint.Y, ref stationLowerPipe, ref offset2);


                elevationLowerPipe = pipeRunProfileLower.ElevationAt(stationLowerPipe);
                elevationUpperPipe = pipeRunProfileUpper.ElevationAt(stationUpperPipe);


                // Constant Parameters
                double pipeUpperSize = _ViewModel.UpperPipe.OuterDiameter;
                double pipeLowerSize = _ViewModel.LowerPipe.OuterDiameter;

                double loweredElevation = elevationUpperPipe - pipeUpperSize - _VerticalClearance;
                double loweredStation1 = stationLowerPipe - _LateralOffset;
                double loweredStation2 = stationLowerPipe + _LateralOffset;
                _LowerElevation = loweredElevation;

                double initialVDiff = pipeUpperSize + _VerticalClearance;


                // Start Transition
                double startDeflection = _MaxDeflection;
                double upperStartAngle;
                double startStation1;
                double startElevation1;
                double finalTransitionStation1;
                double finalTransitionElevation1;
                double startX1 = (initialVDiff / Tangent(startDeflection)) * -1;
                double startX2;

                //_Console.ShowConsole($"{_MaxDeflection}\n{_VerticalClearance}\n{_CrossingLength}");

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
                        if ((upperStartAngle + startDeflection) > _MaxDeflection)
                        {
                            startDeflection = _MaxDeflection - upperStartAngle;
                            startX1 = (initialVDiff / Tangent(startDeflection)) * -1;
                            continue;
                        }
                    }

                    break;
                }


                // End Transition
                double upperEndDeflection = _MaxDeflection;
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

                    //_Console.ShowConsole(intersectionEnd.ToString());

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
                        if ((Math.Abs(upperEndAngle) + startDeflection) > _MaxDeflection)
                        {
                            upperEndDeflection = _MaxDeflection - upperEndAngle;
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

                _ProfileData = new List<(double, double)>();

                _ProfileData.Add(p1);
                _ProfileData.Add(p2);
                _ProfileData.Add(p3);
                _ProfileData.Add(p4);
                _ProfileData.Add(p5);
                _ProfileData.Add(p6);

                SetActualDeflections();
            }
        }


        private double Tangent(double x)
        {
            return Math.Tan(x * (Math.PI / 180));
        }

        private double ArcTangent(double x)
        {
            return Math.Atan(x) * (180 / Math.PI);
        }

        private void SetActualDeflections()
        {
            double d2 = ArcTangent((_ProfileData[1].Item2 - _ProfileData[2].Item2) / (_ProfileData[2].Item1 - _ProfileData[1].Item1));
            double d3 = ArcTangent((_ProfileData[4].Item2 - _ProfileData[3].Item2) / (_ProfileData[4].Item1 - _ProfileData[3].Item1));
            double d1 = ArcTangent((_ProfileData[1].Item2 - _ProfileData[0].Item2) / (_ProfileData[1].Item1 - _ProfileData[0].Item1)) + d2;
            double d4 = d3 - ArcTangent((_ProfileData[5].Item2 - _ProfileData[4].Item2) / (_ProfileData[5].Item1 - _ProfileData[4].Item1));

            _ActualDeflections = new List<double> { d1, d2, d3, d4,};

        }

    }
}
