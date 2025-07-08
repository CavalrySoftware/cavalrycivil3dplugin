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



        #region << BINDING PROPERTIES >>
        private string _ReferenceClearCover = 0.5.ToString("0.00");
        public string ReferenceClearCover
        {
            get
            {
                return _ReferenceClearCover;
            }
            set
            {
                _ReferenceClearCover = value;
                SetCurrentField(() => ReferenceClearCover, v => ReferenceClearCover = v, nameof(ReferenceClearCover));
                OnPropertyChanged(nameof(ReferenceClearCover));
            }
        }


        private string _ReferenceClearDepth = 0.5.ToString("0.00");
        public string ReferenceClearDepth
        {
            get
            {
                return _ReferenceClearDepth;
            }
            set
            {
                _ReferenceClearDepth = value;
                SetCurrentField(() => ReferenceClearDepth, v => ReferenceClearDepth = v, nameof(ReferenceClearDepth));
                OnPropertyChanged(nameof(ReferenceClearDepth));
            }
        }



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
                SetCurrentField(() => VerticalClearance, v => VerticalClearance = v, nameof(VerticalClearance));
                OnPropertyChanged(nameof(VerticalClearance)); 
            } 
        }


        private string _CrossingLengthStart = 1.0.ToString("0.00");
        public string CrossingLengthStart
        {
            get
            {
                return _CrossingLengthStart;
            }
            set
            {
                _CrossingLengthStart = value;
                if (_IsSymmetrical) CrossingLengthEnd = value;
                SetCurrentField(() => CrossingLengthStart, v => CrossingLengthStart = v, nameof(CrossingLengthStart));
                OnPropertyChanged(nameof(CrossingLengthStart));
            }
        }


        private string _CrossingLengthEnd = 1.0.ToString("0.00");
        public string CrossingLengthEnd
        {
            get
            {
                return _CrossingLengthEnd;
            }
            set
            {
                _CrossingLengthEnd = value;
                SetCurrentField(() => CrossingLengthEnd, v => CrossingLengthEnd = v, nameof(CrossingLengthEnd));
                OnPropertyChanged(nameof(CrossingLengthEnd));
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
                SetCurrentField(() => MaxDeflection, v => MaxDeflection = v, nameof(MaxDeflection));
                OnPropertyChanged(nameof(MaxDeflection));
            }
        }


        private string _Status;
        public string Status
        {
            get { return _Status; }
            set { _Status = value; OnPropertyChanged(nameof(Status)); }
        }
        #endregion



        #region << BINDING BOOLEAN PROPERTIES >>
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


        private bool _IsSymmetrical = true;
        public bool IsSymmetrical
        {
            get { return _IsSymmetrical; }
            set 
            { 
                _IsSymmetrical = value;
                IsNotSymmetrical = !value;
                OnPropertyChanged(nameof(IsSymmetrical)); 
            }
        }


        private bool _IsNotSymmetrical = false;
        public bool IsNotSymmetrical
        {
            get { return _IsNotSymmetrical; }
            set { _IsNotSymmetrical = value; OnPropertyChanged(nameof(IsNotSymmetrical)); }
        }


        private bool _IsNotPipeReference = true;

        public bool IsNotPipeReference
        {
            get { return _IsNotPipeReference; }
            set { _IsNotPipeReference = value; OnPropertyChanged(nameof(IsNotPipeReference)); }
        }


        private NotificationMethod _selectedMethod;
        public NotificationMethod SelectedMethod
        {
            get => _selectedMethod;
            set
            {
                _selectedMethod = value;
            }
        }
        #endregion



        #region << BINDING SELECTED PROPERTIES >>
        private ObjectReferenceModel _SelectedObjectReference;
        public ObjectReferenceModel SelectedObjectReference
        {
            get { return _SelectedObjectReference; }
            set { _SelectedObjectReference = value; OnPropertyChanged(nameof(SelectedObjectReference)); }
        }
        #endregion



        #region << MODELS DEFINITION >>
        private LowerPipeMainModel _LowerPipeMainModel;
        public LowerPipeMainModel LowerPipeMainModel_ => _LowerPipeMainModel;
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
        public bool CanApply()
        {
            return true;
            //return _ValidSelection && LowerAnalysisModel.ValidRequirements;
        }

        private Func<string> _GetCurrentField;
        private Action<string> _SetCurrentField;
        public string CurrentFieldName { get; private set; }

        private string CurrentFieldValue
        {
            get => _GetCurrentField?.Invoke();
            set => _SetCurrentField?.Invoke(value);
        }


        private void SetCurrentField(Func<string> getter, Action<string> setter, string propertyName)
        {
            _GetCurrentField = getter;
            _SetCurrentField = value =>
            {
                setter(value);
                OnPropertyChanged(propertyName);
            };
            CurrentFieldName = propertyName;
        }
        #endregion



        #region << WINDOW ACTIONS >>
        public Action CloseAction { get; set; }
        public Action HideAction { get; set; }
        public Action ShowAction { get; set; }
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



        #region << APPLY ACTIONS >>
        public enum NotificationMethod
        {
            ModifyPipe,
            ProfileRange,
            ProfileRun
        }
        #endregion



        #region << CONSTRUCTOR >>
        public LowerPipeMainViewModel()
        {
            InitializeModels();
            InitializedSystem();
            InitializeCommands();
        }
        #endregion



        #region << INITIALIZE FUNCTIONS >>

        private void InitializeModels()
        {
            _LowerPipeMainModel = new LowerPipeMainModel(this);
        }


        private void InitializedSystem()
        {
            _SelectedObjectReference = _LowerPipeMainModel.ObjectReferenceCollection.ObjectReferences.FirstOrDefault();
        }


        private void InitializeCommands()
        {
            _PickPressurePipes = new PickPressurePipes(this);
            _ApplyProfileCommand = new ApplyProfileCommand(this);
            _ApplyCommand = new RelayCommand(_ApplyProfileCommand.Apply, CanApply);
            _CancelCommand = new CancelCommand(this);
            _OkCommand = new OkCommand(this);
        }


        #endregion



        #region << SYSTEM FUNCTIONS >>

        public void EnterValue()
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                Keyboard.ClearFocus();
                FormatValues();

                if (LowerPipeMainModel_.Loaded) LowerPipeMainModel_.Calculate();
            }
        }


        private void FormatValues()
        {
            if (double.TryParse(CurrentFieldValue, out double value))
            {
                CurrentFieldValue = $"{value:0.00}";
            }
        }


        public void InputLostFocus()
        {
            FormatValues();
            if (LowerPipeMainModel_.Loaded) LowerPipeMainModel_.Calculate();
        }

        public void ClosingWindow()
        {
            if (ClosedByXButton)
            {
                _LowerPipeMainModel.CloseModel();
            }
        }

        #endregion
    }
}
