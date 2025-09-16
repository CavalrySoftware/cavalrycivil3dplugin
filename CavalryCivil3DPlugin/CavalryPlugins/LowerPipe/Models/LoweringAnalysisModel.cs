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

        #region << CLASS PARAMETER DEFINITIONS >>
        private LowerPipeMainViewModel _ViewModel;
        private LowerPipeMainModel _MainModel;
        private Document _AutocadDocument;
        #endregion

        #region BOOLEAN PROPERTIES
        private bool _AnalysisValid = true;
        public bool AnalysisValid => _AnalysisValid;

        private string _LoweringEnd;
        public string LoweringEnd => _LoweringEnd;
        #endregion


        #region << CALCULATED PROPERTIES >>
        private Point2d _IntersectionPoint;
        public Point2d IntersectionPoint => _IntersectionPoint;

        private double _MainPipeInitialCoverFromGround;
        public double MainPipeInitialCoverFromGround => _MainPipeInitialCoverFromGround;

        private double _CalculatedCover;
        public double CalculatedCover => _CalculatedCover;

        private double _CalculatedDepth;
        public double CalculatedDepth => _CalculatedDepth;

        private double _CalculatedGroundElevation;
        public double CalculatedGroundElevation => _CalculatedGroundElevation;

        private double _StationOrigin;
        public double StationOrigin => _StationOrigin;
        #endregion


        public LoweringAnalysisModel(LowerPipeMainModel _mainModel)
        {
            _ViewModel = _mainModel.ViewModel;
            _MainModel = _mainModel;
            _AutocadDocument = _mainModel.AutocadDocument;
        }


        public void Analyze()
        {
            string referenceName = _ViewModel.SelectedObjectReference.ReferenceName;

            switch (referenceName)
            {
                case "Pressure Pipe":
                    AnalyzeByPressurePipeReference();
                    break;

                case "Line":
                    AnalyzeByLineReference(); 
                    break;

                case "Polyline":
                    AnalyzeByPolylineReference();
                    break;

                case "End of Pipe Run":
                    AnalyzeByEndofPipeReference();
                    break;
            }
        }


        private void AnalyzeByPressurePipeReference()
        {
            PressurePipeModel upperPipe = _ViewModel.SelectedObjectReference.PressurePipeReference;
            PressurePipeModel lowerPipe = _MainModel.LowerPipe;


            _IntersectionPoint = _PressurePipe.GetIntersectionPoint(_AutocadDocument, lowerPipe.ObjectId_, upperPipe.ObjectId_);
            AnalyzeIntersectionProperties(_IntersectionPoint, _isPipeReference: true);
            
            // Send Back to ViewModel
            _ViewModel.ReferenceClearCover = $"{_CalculatedCover: 0.00}";
            _ViewModel.ReferenceClearDepth = $"{_CalculatedDepth: 0.00}";
            _ViewModel.IsNotPipeReference = false;
        }


        private void AnalyzeByEndofPipeReference()
        {
            PressurePipeModel referencePipe = _ViewModel.SelectedObjectReference.PressurePipeReference;
            PressurePipeModel lowerPipe = _MainModel.LowerPipe;


            using (Transaction tr = _AutocadDocument.Database.TransactionManager.StartTransaction())
            {
                PressurePipe referencePressurePipe = tr.GetObject(referencePipe.ObjectId_, OpenMode.ForRead) as PressurePipe;
                PressurePipe lowerPressurePipe = tr.GetObject(lowerPipe.ObjectId_, OpenMode.ForRead) as PressurePipe;

                Point3d referencePoint = referencePressurePipe.EndPoint;

                Point3d mainPipeStartPoint = lowerPressurePipe.StartPoint;
                Point3d mainPipeEndPoint = lowerPressurePipe.EndPoint;

                double distanceStart = mainPipeStartPoint.DistanceTo(referencePoint);
                double distanceEnd = mainPipeEndPoint.DistanceTo(referencePoint);


                if ((distanceStart < distanceEnd) && lowerPressurePipe.StartConnection.Open)
                {
                    _LoweringEnd = "start";
                    _IntersectionPoint = new Point2d(lowerPressurePipe.StartPoint.X, lowerPressurePipe.StartPoint.Y);
                    _ViewModel.LowerPipeMainModel_.LoggerModel_.SetLogMessage("Pipe Alignment is to be lowered at Start Station to be vertically aligned with the reference pipe.");
                    _AnalysisValid = true;  
                }

                else if ((distanceEnd  < distanceStart) && lowerPressurePipe.EndConnection.Open)
                {
                    _LoweringEnd = "end";
                    _IntersectionPoint = new Point2d(lowerPressurePipe.EndPoint.X, lowerPressurePipe.EndPoint.Y);
                    _ViewModel.LowerPipeMainModel_.LoggerModel_.SetLogMessage("Pipe Alignment is to be lowered at End Station to be vertically aligned with the reference pipe.");
                    _AnalysisValid = true;
                }

                else
                {
                    _AnalysisValid = false;
                    _ViewModel.LowerPipeMainModel_.LoggerModel_.SetErrorMessage("Invalid objects. The connection of the end of pipe to be lowered must be open at the point of the reference Pipe.");
                    return;
                }

                AnalyzeIntersectionProperties(_IntersectionPoint, _isPipeReference: true);

                // Send Back to ViewModel
                _ViewModel.ReferenceClearCover = $"{_CalculatedCover:0.00}";
                _ViewModel.ReferenceClearDepth = $"{referencePressurePipe.OuterDiameter:0.00}";
                _ViewModel.VerticalClearance = $"{0:0.00}";
                _ViewModel.IsNotPipeReference = false;

            } 
        }



        private void AnalyzeByLineReference()
        {
            ObjectId lineReferenceId = _ViewModel.SelectedObjectReference.ObjectId_;
            PressurePipeModel lowerPipe = _MainModel.LowerPipe;

            
            _IntersectionPoint = _PressurePipe.GetIntersectionPointToLine(_AutocadDocument, lowerPipe.ObjectId_, lineReferenceId);
            AnalyzeIntersectionProperties(_IntersectionPoint);

            // Send Back to ViewModel
            _ViewModel.ReferenceClearCover = $"{0.5:0.00}";
            _ViewModel.ReferenceClearDepth = $"{0.5:0.00}";
            _ViewModel.IsNotPipeReference = true;
        }


        private void AnalyzeByPolylineReference()
        {
            var endPoints = _ViewModel.SelectedObjectReference.EndPoints3d;
            PressurePipeModel lowerPipe = _MainModel.LowerPipe;

            _IntersectionPoint = _PressurePipe.GetIntersectionPointToLinePoints(_AutocadDocument, lowerPipe.ObjectId_, endPoints);
            AnalyzeIntersectionProperties(_IntersectionPoint);

            // Send Back to ViewModel
            _ViewModel.ReferenceClearCover = $"{0.5:0.00}";
            _ViewModel.ReferenceClearDepth = $"{0.5:0.00}";
            _ViewModel.IsNotPipeReference = true;
        }


        private void AnalyzeIntersectionProperties(Point2d _intersectionPoint, bool _isPipeReference = false)
        {
            PressurePipeModel mainPipe = _MainModel.LowerPipe;

            using (Transaction tr = _AutocadDocument.Database.TransactionManager.StartTransaction())
            {
                Alignment pipeRunAlignmentLower = tr.GetObject(mainPipe.RunAlignmentId, OpenMode.ForRead) as Alignment;
                Profile pipeRunProfileLower = tr.GetObject(mainPipe.RunProfileId, OpenMode.ForRead) as Profile;
                Profile groundProfile = tr.GetObject(mainPipe.GroundProfileId, OpenMode.ForRead) as Profile;


                double stationLowerPipe = 0;
                double offset2 = 0;
                double elevationLowerPipe;

                double stationGround = 0;
                double offset3 = 0;

                pipeRunAlignmentLower.StationOffset(_intersectionPoint.X, _intersectionPoint.Y, ref stationLowerPipe, ref offset2);
                pipeRunAlignmentLower.StationOffset(_intersectionPoint.X, _intersectionPoint.Y, ref stationGround, ref offset3);

                elevationLowerPipe = pipeRunProfileLower.ElevationAt(stationLowerPipe);
                double elevationGround = groundProfile.ElevationAt(stationGround);

                _MainPipeInitialCoverFromGround = elevationGround - elevationLowerPipe;
                _CalculatedGroundElevation = elevationGround;
                _StationOrigin = stationLowerPipe;

                if (_isPipeReference)
                {
                    PressurePipeModel upperPipe = _ViewModel.SelectedObjectReference.PressurePipeReference;
                    Alignment pipeRunAlignmentUpper = tr.GetObject(upperPipe.RunAlignmentId, OpenMode.ForRead) as Alignment;
                    Profile pipeRunProfileUpper = tr.GetObject(upperPipe.RunProfileId, OpenMode.ForRead) as Profile;

                    double stationUpperPipe = 0;
                    double offset1 = 0;
                    pipeRunAlignmentUpper.StationOffset(_IntersectionPoint.X, _IntersectionPoint.Y, ref stationUpperPipe, ref offset1);

                    double elevationUpperPipe = pipeRunProfileUpper.ElevationAt(stationUpperPipe);
                    _CalculatedDepth = upperPipe.OuterDiameter;
                    _CalculatedCover = elevationGround - elevationUpperPipe;
                }
            }
        }
    }
}
