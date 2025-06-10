using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.Settings;
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Commands;
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Models;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.ViewModels
{
    public class LowerPipeMainViewModel : INotifyPropertyChanged
    {

        #region << INITIALIZATION FOR INOTIFYPROPERYCHANGED >>
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        #endregion



        #region << CAD DEPENDENCY PROPERTIES >>
        //    Civil3DDocument = CivilApplication.ActiveDocument;
        private Document _AutocadDocument;
        public Document AutocadDocument
        {
            get { return _AutocadDocument; }
        }

        private CivilDocument _Civil3DDocument;
        public CivilDocument Civil3DDocument
        {
            get { return _Civil3DDocument; }
        }

        public DocumentLock MainDocumentLock;
        public Transaction MainTransaction;
        #endregion



        #region << FRONTEND PROPERTIES >>
        private string _VerticalClearance = 0.5.ToString("0.00");
        public string VerticalClearance 
        { 
            get 
            { 
                return _VerticalClearance; 
            } 
            set 
            { 
                _VerticalClearance = value;
                OnPropertyChanged(nameof(VerticalClearance)); 
            } 
        }


        private string _CrossingLength = 2.0.ToString("0.00");
        public string CrossingLength
        {
            get
            {
                return _CrossingLength;
            }
            set
            {
                _CrossingLength = value;
                OnPropertyChanged(nameof(CrossingLength));
            }
        }


        private string _MaxDeflection = 7.0.ToString("0.00");
        public string MaxDeflection
        {
            get
            {
                return _MaxDeflection;
            }
            set
            {
                _MaxDeflection = value;
                OnPropertyChanged(nameof(MaxDeflection));
            }
        }


        private bool _ModifyPipe = true;
        public bool ModifyPipe
        {
            get { return _ModifyPipe; }
            set { _ModifyPipe = value; OnPropertyChanged(nameof(ModifyPipe)); }
        }


        private bool _DeleteProfile;
        public bool DeleteProfile
        {
            get { return _DeleteProfile; }
            set { _DeleteProfile = value; OnPropertyChanged(nameof(DeleteProfile)); }
        }


        private string _Status;
        public string Status
        {
            get { return _Status; }
            set { _Status = value; OnPropertyChanged(nameof(Status)); }
        }
        #endregion



        #region << BACKEND AND MODEL PROPERTIES >>
        private PressurePipeModel _UpperPipe;
        public PressurePipeModel UpperPipe { get { return _UpperPipe; } }

        private PressurePipeModel _LowerPipe;
        public PressurePipeModel LowerPipe { get { return _LowerPipe; }} 

        private CanvasModel _CanvasModel;
        public CanvasModel CanvasModel_ { get { return _CanvasModel; }}

        private LoweringAnalysisModel _LowerAnalysisModel;
        public LoweringAnalysisModel LowerAnalysisModel { get { return _LowerAnalysisModel; } }
        #endregion



        #region << COMMANDS DEFINITIONS >>
        private PickPressurePipes _PickPressurePipes;
        public PickPressurePipes PickPressurePipes { get { return _PickPressurePipes; } }

        public ApplyProfileCommand _ApplyProfileCommand;  
        
        private ICommand _ApplyCommand;
        public ICommand ApplyCommand { get { return _ApplyCommand; } }

        private ICommand _CancelCommand;
        public ICommand CancelCommand_ { get { return _CancelCommand; } }

        private ICommand _OkCommand;
        public ICommand OkCommand { get { return _OkCommand; } }
        #endregion



        #region << SYSTEM VARIABLES AND PROPERTIES >>
        private bool _ValidSelection = false;
        public bool ValidSelection { get { return _ValidSelection; } }


        public bool CanApply()
        {
            return _ValidSelection && LowerAnalysisModel.ValidRequirements;
        }


        private bool _InitialEdit = false;

        public bool InitialEdit
        {
            get { return _InitialEdit; }
            set { _InitialEdit = value; }
        }


        public Action CloseAction { get; set; }
        public Action HideAction { get; set; }
        public Action ShowAction { get; set; }

        public List<ObjectId> _NewProfileIds = new List<ObjectId>();

        public bool ClosedByXButton = false;
        #endregion



        #region << CANVAS PROPERTIES >>


        private double _zoom = 1.0;
        public double Zoom
        {
            get => _zoom;
            set { _zoom = value; OnPropertyChanged(nameof(Zoom)); }
        }

        private double _offsetX;
        public double OffsetX
        {
            get => _offsetX;
            set { _offsetX = value; OnPropertyChanged(nameof(OffsetX)); }
        }

        private double _offsetY;
        public double OffsetY
        {
            get => _offsetY;
            set { _offsetY = value; OnPropertyChanged(nameof(OffsetY)); }
        }
        #endregion



        #region << CONSTRUCTOR >>
        public LowerPipeMainViewModel()
        {

            _AutocadDocument = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            _Civil3DDocument = CivilApplication.ActiveDocument;
            MainDocumentLock = _AutocadDocument.LockDocument();
            MainTransaction = _AutocadDocument.Database.TransactionManager.StartTransaction();

            InitializeViewModel();
            InitializeCommands();
            InitializeSelection();
            //var ss = new LowerPipeViewModel();
        }
        #endregion



        #region << INITIALIZE FUNCTIONS >>

        private void InitializeViewModel()
        {
            _UpperPipe = new PressurePipeModel(_AutocadDocument);
            _LowerPipe = new PressurePipeModel(_AutocadDocument);
            _CanvasModel = new CanvasModel(this);
            _LowerAnalysisModel = new LoweringAnalysisModel(this);
        }


        private void InitializeCommands()
        {
            _PickPressurePipes = new PickPressurePipes(this);
            _ApplyProfileCommand = new ApplyProfileCommand(this);
            _ApplyCommand = new RelayCommand(_ApplyProfileCommand.Apply, CanApply);
            _CancelCommand = new CancelCommand(this);
            _OkCommand = new OkCommand(this);
        }


        public void ResetCalculations()
        {

            try
            {
                _LowerAnalysisModel.Analyze();
                _CanvasModel.Update();
                UpdateLogs();
            }

            catch (Exception ex) { _Console.ShowConsole(ex.ToString()); }

        }


        private void InitializeSelection()
        {
            _ValidSelection = _PickPressurePipes.Pick();
            if (_ValidSelection)
            {
                ResetCalculations();
            }
            UpdateLogs();
        }
        #endregion



        #region << SYSTEM FUNCTIONS >>
        private void UpdateLogs()
        {
            if (_ValidSelection)
            {
                _Status =
                    "* Upper Pipe:" +
                    $"\n\t- Network Name:   {_UpperPipe.NetworkName}" +
                    $"\n\t- PipeRun Name:   {_UpperPipe.PipeRunName}" +
                    $"\n\t- Outer Diameter:  {_UpperPipe.OuterDiameter:0.0000} m" +
                    "\n\n" +
                    "* Lower Pipe:" +
                    $"\n\t- Network Name:   {_LowerPipe.NetworkName}" +
                    $"\n\t- PipeRun Name:   {_LowerPipe.PipeRunName}" +
                    $"\n\t- Outer Diameter:  {_LowerPipe.OuterDiameter:0.0000} m";
            }

            else
            {
                _Status = "Invalid pipe selection or selection has been cancelled.";
            }
        }


        public void EnterValue()
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                Keyboard.ClearFocus();
                ResetCalculations();
            }
        }


        public void InputLostFocus()
        {
            ResetCalculations();
        }


        public void DeleteProfiles()
        {
            if (_NewProfileIds.Count > 0)
            {
                using (Transaction tr = AutocadDocument.Database.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId profileId in _NewProfileIds)
                    {
                        Profile profile = tr.GetObject(profileId, OpenMode.ForWrite) as Profile;    
                        profile.Erase();
                    }

                    tr.Commit();
                }
            }
        }


        public void ClosingWindow()
        {
            if (ClosedByXButton)
            {
                MainTransaction.Abort();
                MainTransaction.Dispose();
                MainDocumentLock.Dispose();
                AutocadDocument.Editor.Regen();
            }
        }
        #endregion
    }
}
