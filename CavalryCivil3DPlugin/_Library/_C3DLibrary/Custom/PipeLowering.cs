using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using CavalryCivil3DPlugin._ACADLibrary.Elements;
using CavalryCivil3DPlugin._ACADLibrary.Geometry;
using CavalryCivil3DPlugin._C3DLibrary.Elements;
using CavalryCivil3DPlugin._MathLibrary;
using CavalryCivil3DPlugin.Consoles;
using CavalryCivil3DPlugin.Models.UI;

namespace CavalryCivil3DPlugin._C3DLibrary.Custom
{
    public class PipeLowering
    {
        Document _AutocadDocument;
        CivilDocument _CivilDocument;

        private List<(double, double)>  _ProfileData;
        public List<(double, double)> ProfileData => _ProfileData;

        private List<double> _ActualDeflections;
        public List<double> ActualDeflections => _ActualDeflections;

        private double _MaxDeflection;

        private ObjectId _NewProfileId = ObjectId.Null;
        public ObjectId NewProfileId => _NewProfileId;

        private ObjectId _ReferenceAlignmentId = ObjectId.Null;
        private ObjectId _ReferenceProfileId = ObjectId.Null;   
        
        private bool _ValidCalculation;
        public bool ValidCalculation => _ValidCalculation;

        private double _CrossLengthStart;
        public double CrossLengthStart => _CrossLengthStart;

        private double _CrossLengthEnd;
        public double CrossLengthEnd => _CrossLengthEnd;

        private double _RefCover;
        public double RefCover => _RefCover;

        private double _RefDepth;
        public double RefDepth => _RefDepth;

        private double _VerticalClearance;
        public double VerticalClearance => _VerticalClearance;

        private bool _Initialized = false;
        public bool Initialized => _Initialized;

        private string _Error;
        public string Error => _Error;

        private LoggerViewModel _Logger;

        private bool _ProfileCreated = false;
        public bool ProfileCreated => _ProfileCreated;



        public PipeLowering(Document _autocadDocument, CivilDocument _civil3DDocument, LoggerViewModel _logger = null)
        {
            _AutocadDocument = _autocadDocument;
            _CivilDocument = _civil3DDocument;
            _Logger = _logger == null ? new LoggerViewModel() : _logger;
        }


        public void Generate(
            double _referenceCover,
            double _referenceDepth,
            double _verticalClearance,
            double _crossLengthStart,
            double _crossLengthEnd,
            double _maxDeflection,
            double _groundElevation,
            double _stationOrigin,
            ObjectId _referenceAlignmentId,
            ObjectId _referenceProfileId)
        {

            try
            {
                GenerateProfileData(_referenceCover,
                             _referenceDepth,
                             _verticalClearance,
                             _crossLengthStart,
                             _crossLengthEnd,
                             _maxDeflection,
                             _groundElevation,
                             _stationOrigin,
                             _referenceProfileId);

                _ReferenceAlignmentId = _referenceAlignmentId;
                _ReferenceProfileId = _referenceProfileId;
            }

            catch (Exception ex)
            {
                _ValidCalculation = false;
                _Console.ShowConsole(ex.ToString());    
            }
        }


        public void CreateProfile(bool rangeOnly = false)
        {

            if (_ReferenceAlignmentId != ObjectId.Null && _ReferenceProfileId != ObjectId.Null)
            {
                if (_ProfileCreated && _NewProfileId != ObjectId.Null)
                {
                    using(Transaction tr = _AutocadDocument.Database.TransactionManager.StartTransaction())
                    {
                        Profile currentProfile = tr.GetObject(_NewProfileId, OpenMode.ForWrite) as Profile;
                        currentProfile.Erase();
                        tr.Commit();
                        _AutocadDocument.Editor.Regen();
                    }
                }

                if (rangeOnly)
                {
                    _NewProfileId = _Profile.CreateProfile
                                                            (
                                                            _AutocadDocument,
                                                            _CivilDocument,
                                                            _ReferenceAlignmentId,
                                                            _ReferenceProfileId,
                                                            _ProfileData
                                                            );
                    _AutocadDocument.Editor.Regen();
                }

                else
                {
                    _NewProfileId = _Profile.ReCreateProfile
                                                            (
                                                            _AutocadDocument,
                                                            _CivilDocument,
                                                            _ReferenceAlignmentId,
                                                            _ReferenceProfileId,
                                                            _ProfileData
                                                            );
                    _AutocadDocument.Editor.Regen();
                }
                

                using(Transaction tr = _AutocadDocument.Database.TransactionManager.StartTransaction())
                {
                    Profile newProfile = tr.GetObject(_NewProfileId, OpenMode.ForRead) as Profile;
                    _Logger.SetLogMessage($"Design Profile created in the model.\nProfile name: {newProfile.Name}");
                }
                _ProfileCreated = true;
            }
        }


        private void GenerateProfileData
            (
            double _referenceCover,
            double _referenceDepth,
            double _verticalClearance,
            double _crossLengthStart,
            double _crossLengthEnd,
            double _maxDeflection,
            double _groundElevation,
            double _stationOrigin,
            ObjectId _referenceProfileId
            )
        {

            // Defining General Constant Parameters for the new Profile
            _CrossLengthStart = _crossLengthStart;
            _CrossLengthEnd = _crossLengthEnd;
            _MaxDeflection = _maxDeflection;
            _RefCover = _referenceCover;
            _RefDepth = _referenceDepth;
            _VerticalClearance = _verticalClearance;
            double bottomElevation = _groundElevation - _referenceCover - _referenceDepth - _verticalClearance;
            double mainStation1 = _stationOrigin - _crossLengthStart;
            double mainStation2 = _stationOrigin + _crossLengthEnd;
            
            using (Transaction tr = _AutocadDocument.Database.TransactionManager.StartTransaction())
            {

                // General Constant Parameters
                Profile mainPipe = tr.GetObject(_referenceProfileId, OpenMode.ForRead) as Profile;
                double initialPipeElevation = mainPipe.ElevationAt(_stationOrigin);
                double initialY = _referenceCover + _referenceDepth + _verticalClearance - (_groundElevation - initialPipeElevation);
                _ValidCalculation = true;

                // General Objectic Variable Containers
                (double, double) p1;
                (double, double) p2;
                (double, double) p3 = (mainStation1, bottomElevation);
                (double, double) p4 = (mainStation2, bottomElevation);
                (double, double) p5;
                (double, double) p6;

                (p2, p1) = GetTappingPoints
                    (
                    _referenceProfileId: _referenceProfileId,
                    _initialY: initialY,
                    _maxDeflection: _maxDeflection,
                    _referenceStation: mainStation1,
                    _bottomElevation: bottomElevation,
                    _start : true
                    );

                (p5, p6) = GetTappingPoints
                    (
                    _referenceProfileId: _referenceProfileId,
                    _initialY: initialY,
                    _maxDeflection: _maxDeflection,
                    _referenceStation: mainStation2,
                    _bottomElevation: bottomElevation,
                    _start: false
                    );

                if (_ValidCalculation)
                {
                    _ProfileData = new List<(double, double)>();

                    _ProfileData.Add(p1);
                    _ProfileData.Add(p2);
                    _ProfileData.Add(p3);
                    _ProfileData.Add(p4);
                    _ProfileData.Add(p5);
                    _ProfileData.Add(p6);

                    SetActualDeflections();
                    _Initialized = true;
                }
            }
        }



        private ((double, double), (double, double)) GetTappingPoints(ObjectId _referenceProfileId, double _initialY, double _maxDeflection, double _referenceStation, double _bottomElevation, bool _start)
        {

            if (!_ValidCalculation) return ((0, 0), (0, 0));

            int factor = _start ? -1 : 1;
            (double, double) p1;
            (double, double) p2;

            using (Transaction tr = _AutocadDocument.Database.TransactionManager.StartTransaction())
            {

                Profile mainPipe = tr.GetObject(_referenceProfileId, OpenMode.ForRead) as Profile;

                // Generic Variable Containers
                double deflection;
                double x;
                double y;
                Point2d intersectionPoint = new Point2d();
                (Point2d, Point2d) mainLineProjection;
                (Point2d, Point2d) segmentLine;
                double tapExtension = 0.3 * factor;
                var PVIs = mainPipe.PVIs;
                int totalPVIs = PVIs.Count;
                int index = 0;
                List<((double, double), (double, double))> segments = new List<((double, double), (double, double))>();
                ((double, double), (double, double)) currentSegment;
                int currentSegmentIndex = 0;
                deflection = _maxDeflection;
                double lastSegmentIndex;

                x = _referenceStation + factor;
                y = _Math.Tangent(deflection) + _bottomElevation;
                mainLineProjection = (new Point2d(_referenceStation, _bottomElevation), new Point2d(x, y));

                
                for (int i = 0; i < totalPVIs - 1; i++)
                {
                    ProfilePVI pviStart = PVIs[i];
                    ProfilePVI pviEnd = PVIs[i + 1];
                    segments.Add(((pviStart.Station, pviStart.Elevation),  (pviEnd.Station, pviEnd.Elevation)));
                }

                index = 0;  
                foreach (var segment in segments)
                {
                    double startSegmentStation = segment.Item1.Item1;
                    double endSegmentStation = segment.Item2.Item1;

                    if (_referenceStation > startSegmentStation && _referenceStation < endSegmentStation)
                    {
                        currentSegment = segment;
                        currentSegmentIndex = index;
                        break;
                    };
                    index++;
                }

                lastSegmentIndex = _start ? 0 : segments.Count - 1;

                
                while (true)
                {
                    currentSegment = segments[currentSegmentIndex];
                    double startStation = currentSegment.Item1.Item1;
                    double endStation = currentSegment.Item2.Item1;
                    segmentLine = (new Point2d(currentSegment.Item1.Item1, currentSegment.Item1.Item2), new Point2d(currentSegment.Item2.Item1, currentSegment.Item2.Item2));
                    intersectionPoint = Lines.GetIntersectionPoint(mainLineProjection, segmentLine);
                    
                    if (intersectionPoint.X > startStation && intersectionPoint.X < endStation)
                    {
                        _ValidCalculation = true;
                        break;
                    };

                    currentSegmentIndex = currentSegmentIndex + factor;
                    
                    if (_start)
                    {
                        if (currentSegmentIndex < lastSegmentIndex)
                        {
                            _ValidCalculation = false;
                            _Logger.SetErrorMessage("Invalid Calculation.\nDeflection is too small or the lowering is too deep. Can't satisfied input requirements.");
                            break;
                        };
                    }
                    else
                    {
                        if (currentSegmentIndex > lastSegmentIndex)
                        {
                            _ValidCalculation = false;
                            _Logger.SetErrorMessage("Invalid Calculation.\nDeflection is too small or the lowering is too deep. Can't satisfied input requirements.");
                            break;
                        }
                    }
                }

                
                if (_ValidCalculation)
                {
                    
                    double tappingAngle = _Math.ArcTangent((currentSegment.Item1.Item2 - currentSegment.Item2.Item2) / (currentSegment.Item1.Item1 - currentSegment.Item2.Item1));
                    
                    if (Math.Abs(tappingAngle) > _MaxDeflection)
                    {
                        _ValidCalculation = false;
                        _Logger.SetErrorMessage("Invalid Calculation.\nDeflection is too small or the lowering is too deep. Can't satisfied input requirements.\nThe profile grade of the pipe is greater than the lowering deflection");
                    }


                    else
                    {
                        
                        if (_start && tappingAngle > 0 && (_maxDeflection + tappingAngle) > _MaxDeflection)
                        {
                            _maxDeflection = _maxDeflection - tappingAngle;
                            return GetTappingPoints(_referenceProfileId, _initialY, _maxDeflection, _referenceStation, _bottomElevation, _start);
                        }

                        else if (!_start && tappingAngle < 0 && (_maxDeflection + tappingAngle) > _MaxDeflection)
                        {
                            _maxDeflection = _maxDeflection - Math.Abs(tappingAngle);
                            return GetTappingPoints(_referenceProfileId, _initialY, _maxDeflection, _referenceStation, _bottomElevation, _start);
                        }
                    
                        p1 = (intersectionPoint.X, intersectionPoint.Y);
                        double st = intersectionPoint.X + tapExtension;
                        double el = mainPipe.ElevationAt(st);
                        p2 = (st, el);

                        _Logger.SetLogMessage("New Profile data generated.");
                        return (p1, p2);
                    }
                }
                
                
                return ((0, 0), (0, 0));
            }
        }



        private void SetActualDeflections()
        {
            double d2 = _Math.ArcTangent((_ProfileData[1].Item2 - _ProfileData[2].Item2) / (_ProfileData[2].Item1 - _ProfileData[1].Item1));
            double d3 = _Math.ArcTangent((_ProfileData[4].Item2 - _ProfileData[3].Item2) / (_ProfileData[4].Item1 - _ProfileData[3].Item1));
            double d1 = _Math.ArcTangent((_ProfileData[1].Item2 - _ProfileData[0].Item2) / (_ProfileData[1].Item1 - _ProfileData[0].Item1)) + d2;
            double d4 = d3 - _Math.ArcTangent((_ProfileData[5].Item2 - _ProfileData[4].Item2) / (_ProfileData[5].Item1 - _ProfileData[4].Item1));

            _ActualDeflections = new List<double> { d1, d2, d3, d4, };
        }
    }
}
        

   
