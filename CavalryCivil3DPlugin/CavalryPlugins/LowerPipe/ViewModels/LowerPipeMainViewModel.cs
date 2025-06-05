using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Commands;
using CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Models;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.ViewModels
{
    public class LowerPipeMainViewModel : INotifyPropertyChanged
    {
        #region << INITIALIZATION FOR INOTIFYPROPERYCHANGED >>
        //public event PropertyChangedEventHandler PropertyChanged;

        //private void OnPropertyChanged(string propertyName)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

        //public event PropertyChangedEventHandler PropertyChanged;
        //private void OnPropertyChanged([CallerMemberName] string propName = null)
        //    => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        #endregion



        #region << CAD DEPENDENCY PROPERTIES >>
        //    Civil3DDocument = CivilApplication.ActiveDocument;
        private Document _AutocadDocument = Application.DocumentManager.MdiActiveDocument;
        public Document AutocadDocument
        {
            get { return _AutocadDocument; }
        }

        private CivilDocument _Civil3DDocument = CivilApplication.ActiveDocument;
        public CivilDocument Civil3DDocument
        {
            get { return _Civil3DDocument; }
        }
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



        #region << BACKEND PROPERTIES >>
        PressurePipeModel UpperPipe;
        PressurePipeModel LowerPipe;

        #endregion



        #region << COMMAND INTERFACES >>
        private PickPressurePipes _PickPressurePipes;
        public PickPressurePipes PickPressurePipes { get { return _PickPressurePipes; } }

        #endregion



        #region << SYSTEM VARIABLES AND PROPERTIES >>
        private bool _ValidSelection;
        public bool ValidSelection { get { return _ValidSelection; } }

        #endregion



        #region << CANVAS PROPERTIES >>

        public ObservableCollection<ShapeViewModel> Shapes { get; } = new ObservableCollection<ShapeViewModel>();

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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        #endregion



        #region << CONSTRUCTOR >>
        public LowerPipeMainViewModel()
        {
            InitializeCommands();
            InitializeSelection();
            InitilializeCanvas();
        }
        #endregion


        #region << INITIALIZE FUNCTIONS >>

        private void InitializeSelection()
        {
            _ValidSelection = _PickPressurePipes.Pick();
            UpdateLogs();
        }

        private void InitializeCommands()
        {
            _PickPressurePipes = new PickPressurePipes(this);
        }

        private void InitilializeCanvas()
        {
            Shapes.Add(new ShapeViewModel { X = 100, Y = 100 });
            Shapes.Add(new ShapeViewModel { X = 200, Y = 150, Fill = Brushes.Red });
        }

        #endregion


        #region << SYSTEM FUNCTIONS >>
        private void UpdateLogs()
        {
            if (_ValidSelection)
            {
                _Status =
                    "* Upper Pipe:" +
                    $"\n\t- Network Name:   {UpperPipe.NetworkName}" +
                    $"\n\t- PipeRun Name:   {UpperPipe.PipeRunName}" +
                    $"\n\t- Outer Diameter:  {UpperPipe.OuterDiameter:0.0000} m" +
                    "\n\n" +
                    "* Lower Pipe:" +
                    $"\n\t- Network Name:   {LowerPipe.NetworkName}" +
                    $"\n\t- PipeRun Name:   {LowerPipe.PipeRunName}" +
                    $"\n\t- Outer Diameter:  {LowerPipe.OuterDiameter:0.0000} m";
            }

            else
            {
                _Status = "Invalid pipe selection or selection has been cancelled.";
            }
        }

        public void SetPressurePipes(ObjectId _upperPipeId, ObjectId _lowerPipeId)
        {
            UpperPipe = new PressurePipeModel(_upperPipeId, AutocadDocument);
            LowerPipe = new PressurePipeModel(_lowerPipeId, AutocadDocument);
        }
        #endregion


    }
}
