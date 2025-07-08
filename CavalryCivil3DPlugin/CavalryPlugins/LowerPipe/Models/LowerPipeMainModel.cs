using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using CavalryCivil3DPlugin._C3DLibrary.Custom;
using CavalryCivil3DPlugin._C3DLibrary.Elements;
using CavalryCivil3DPlugin.C3DLibrary.Selection;
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.ViewModels;
using CavalryCivil3DPlugin.Consoles;
using CavalryCivil3DPlugin.Models.UI;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Models
{
    public class LowerPipeMainModel
    {

        #region << CAD DEPENDENCIES >>
        private Document _AutocadDocument;
        public Document AutocadDocument { get { return _AutocadDocument; } }

        private CivilDocument _Civil3DDocument;
        public CivilDocument Civil3DDocument { get { return _Civil3DDocument; } }

        public DocumentLock MainDocumentLock;
        public Transaction MainTransaction;
        #endregion



        #region << INPUT PROPERTIES >>
        private double _ReferenceCover;
        public double ReferenceCover => _ReferenceCover;

        private double _ReferenceClearDepth;
        public double ReferenceClearDepth => _ReferenceClearDepth;

        private double _VerticalClearance;
        public double VerticalClearance => _VerticalClearance;

        private double _CrossingLengthStart;
        public double CrossingLengthStart => _CrossingLengthStart;

        private double _CrossingLengthEnd;
        public double CrossingLengthEnd => CrossingLengthEnd;

        private double _MaxDeflection;
        public double MaxDeflection => _MaxDeflection;
        #endregion



        #region << BOOLEAN PROPERTIES >>
        private bool _ValidRequirements;
        public bool ValidRequirements => _ValidRequirements;

        private bool _Loaded = false;
        public bool Loaded => _Loaded;
        #endregion



        #region << SYSTEM VARIABLES >>
        private PressurePipeModel _LowerPipe;
        public PressurePipeModel LowerPipe => _LowerPipe;

        private bool _ValidSelection = false;
        public bool ValidSelection { get { return _ValidSelection; } }
        #endregion



        #region << ERROR HANDLING AND LOGGIN >>
        private bool _Error;
        public bool Error => _Error;
        private string _ErrorMessage;
        public string ErrorMessage 
        { 
            get 
            { 
                return _ErrorMessage; 
            }
            set
            {
                _ErrorMessage = value;
                _LoggerModel.SetErrorMessage(value);
                _Error = true;
            }
        }


        private string _LogMessage;
        public string LogMessage
        {
            get
            {
                return _LogMessage;
            }
            set
            {
                _LogMessage = value;
                _LoggerModel.SetLogMessage(value);
            }
        }
        #endregion



        #region << MODEL PROPERTIES >>
        private LowerPipeMainViewModel _ViewModel;
        public LowerPipeMainViewModel ViewModel => _ViewModel;

        private ObjectReferenceCollectionModel _ObjectReferenceCollection;
        public ObjectReferenceCollectionModel ObjectReferenceCollection => _ObjectReferenceCollection;

        private LoweringAnalysisModel _LoweringAnalysisModel;
        public LoweringAnalysisModel LoweringAnalysisModel => _LoweringAnalysisModel;

        private PipeLowering _PipeLowering;
        public PipeLowering PipeLowering_ => _PipeLowering;

        private CanvasModel _CanvasModel;
        public CanvasModel CanvasModel_ => _CanvasModel;

        private LoggerViewModel _LoggerModel = new LoggerViewModel() { MainMessage = "Select Pressure Pipe and reference object in the model."};
        public LoggerViewModel LoggerModel_ => _LoggerModel;
        #endregion



        #region << CONSTRUCTOR >>
        public LowerPipeMainModel(LowerPipeMainViewModel _viewmodel)
        {
            _ViewModel = _viewmodel;
            InitializeCadEnviroment();
            InitializeSubModels();
        }
        #endregion


        public void DeleteProfiles()
        {
            //if (_NewProfileIds.Count > 0)
            //{
            //    using (Transaction tr = AutocadDocument.Database.TransactionManager.StartTransaction())
            //    {
            //        foreach (ObjectId profileId in _NewProfileIds)
            //        {
            //            Profile profile = tr.GetObject(profileId, OpenMode.ForWrite) as Profile;
            //            profile.Erase();
            //        }

            //        tr.Commit();
            //    }
            //}
        }

        public void CloseModel()
        {
            MainTransaction.Abort();
            MainTransaction.Dispose();
            MainDocumentLock.Dispose();
            AutocadDocument.Editor.Regen();
        }


        private void InitializeCadEnviroment()
        {
            _AutocadDocument = Application.DocumentManager.MdiActiveDocument;
            _Civil3DDocument = CivilApplication.ActiveDocument;
            MainDocumentLock = _AutocadDocument.LockDocument();
            MainTransaction = _AutocadDocument.Database.TransactionManager.StartTransaction();
        }


        private void InitializeSubModels()
        {
            _ObjectReferenceCollection = new ObjectReferenceCollectionModel(_AutocadDocument);
            _LowerPipe = new PressurePipeModel(_AutocadDocument);
            _LoweringAnalysisModel = new LoweringAnalysisModel(this);
            _PipeLowering = new PipeLowering(_AutocadDocument, Civil3DDocument, LoggerModel_);
            _CanvasModel = new CanvasModel(this);
        }


        private void PickObjects()
        {
            ObjectId _LowerPipeId = C3DObjectSelection.PickPressurePipe(_AutocadDocument, "Pressure Pipe to be lowered");

            if (!(_LowerPipeId == ObjectId.Null))
            {
                _ViewModel.SelectedObjectReference.SetObject();

                if (_ViewModel.SelectedObjectReference.ObjectId_ != ObjectId.Null)
                {
                    _LowerPipe.SetPipe(_LowerPipeId);
                    _ValidSelection = true;
                    return;
                }
            } 

            _ValidSelection = false;
        }


        public void SetObjects()
        {
            try
            {
                PickObjects();
            
                if (_ValidSelection)
                {
                    LoweringAnalysisModel.Analyze();
                    _Loaded = true;
                    Calculate();
                }
            }

            catch (Exception ex) 
            {
                _LoggerModel.SetErrorMessage("Invalid reference.\nIntersection point between pipe and reference object is not valid.");
                _ValidSelection = false;
            }
        }


        public void Calculate()
        {
            GetDataRequirements();
            if (_ValidRequirements)
            {
                try
                {
                    GenerateProfileData();
                    if (_PipeLowering.ValidCalculation)
                    {
                        _CanvasModel.Update();
                    }
                    else
                    {
                        _CanvasModel.Erase();
                        //ErrorMessage = "Invalid Calculation" + "\n" + ErrorMessage;
                    }
                }
                catch (Exception ex) { _Console.ShowConsole(ex.ToString()); }
            }
        }


        public void CreateProfile(bool _rangeOnly = false)
        {
            if (_PipeLowering.Initialized)
            {
                _PipeLowering.CreateProfile(_rangeOnly);
            }
        }



        public void FollowPipe()
        {
            if (_PipeLowering.NewProfileId != ObjectId.Null)
            {
                _PressurePipe.SetProfileToPipeRun(_AutocadDocument, _LowerPipe.ObjectId_, _PipeLowering.NewProfileId);
            }
        }


        private void GenerateProfileData()
        {
             _PipeLowering.Generate
                (
                _referenceCover: _ReferenceCover,
                _referenceDepth: _ReferenceClearDepth,
                _verticalClearance: _VerticalClearance,
                _crossLengthStart: _CrossingLengthStart,
                _crossLengthEnd: _CrossingLengthEnd,
                _maxDeflection: _MaxDeflection,
                _groundElevation: _LoweringAnalysisModel.CalculatedGroundElevation,
                _stationOrigin: _LoweringAnalysisModel.StationOrigin,
                _referenceAlignmentId: _LowerPipe.RunAlignmentId,
                _referenceProfileId: _LowerPipe.RunProfileId
                );
        }


        private void GetDataRequirements()
        {
            bool _nonZeroValues = true;
            bool _nonZeroDrop = true;
            bool _numericValues = 
                (
                double.TryParse(_ViewModel.ReferenceClearCover, out _ReferenceCover) &&
                double.TryParse(_ViewModel.ReferenceClearDepth, out _ReferenceClearDepth) &&
                double.TryParse(_ViewModel.VerticalClearance, out _VerticalClearance) &&
                double.TryParse(_ViewModel.CrossingLengthStart, out _CrossingLengthStart) &&
                double.TryParse(_ViewModel.CrossingLengthEnd, out _CrossingLengthEnd) &&
                double.TryParse(_ViewModel.MaxDeflection, out _MaxDeflection)
                );

            if (_numericValues)
            {
                _nonZeroValues = (_MaxDeflection != 0 && _CrossingLengthEnd != 0 && _CrossingLengthStart != 0);
                _nonZeroDrop = (_ReferenceCover + _ReferenceClearDepth + _VerticalClearance) > _LoweringAnalysisModel.MainPipeInitialCoverFromGround;

                _ValidRequirements = (_nonZeroValues && _nonZeroDrop);
                if (_ValidRequirements)
                {
                    return;
                }
            }

            else
            {
                ErrorMessage = "Invalid input. Make sure the input parameter is a valid number or not empty.";
                _CanvasModel.Erase();
            }

            if (!_nonZeroValues)
            {
                ErrorMessage = "Crossing Lengths and Max Deflection must not be zero.";
                _CanvasModel.Erase();
            }

            else if (!_nonZeroDrop)
            {
                ErrorMessage = "Total drop or lowering must not be zero. Increase the value of Reference Clear Cover, Reference Clear Depth, or Clear Space from Reference";
                _CanvasModel.Erase();
            }

            _ValidRequirements = false;
        }
    }
}
