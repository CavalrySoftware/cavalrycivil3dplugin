using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CavalryCivil3DPlugin._Library._CustomModule;
using CavalryCivil3DPlugin.CavalryPlugins.Tag.Commands;
using CavalryCivil3DPlugin.CavalryPlugins.Tag.Models;
using CavalryCivil3DPlugin.Consoles;
using CavalryCivil3DPlugin.Models.CADObject;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Autodesk.AutoCAD.EditorInput;

namespace CavalryCivil3DPlugin.CavalryPlugins.Tag.ViewModels
{
    public class TagMainViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));



        #region RADIOBUTTON OPTIONS
        public enum NotificationMethod
        {
            Single,
            Multiple,
            All
        }

        private NotificationMethod _SelectedMethod;
        public NotificationMethod SelectedMethod
        {
            get => _SelectedMethod;
            set
            {
                _SelectedMethod = value;
                OnPropertyChanged(nameof(SelectedMethod));

                switch (_SelectedMethod)
                {
                    case NotificationMethod.Single:
                        Multiple = false;
                        break;
                    case NotificationMethod.Multiple:
                        Multiple = true;
                        break;
                    case NotificationMethod.All:
                        Multiple = true;
                        break;
                }
            }
        }

        #endregion


        #region << WINDOW ACTIONS PROPERTIES >>
        public Action CloseAction { get; set; }
        public Action HideAction { get; set; }
        public Action ShowAction { get; set; }
        public bool ClosedByXButton = false;
        #endregion


        #region MODEL PROPERTIES DEFINITION
        private TagMainModel _TagMainModel;
        public TagMainModel TagMainModel => _TagMainModel;
        #endregion


        #region SELECTED BINDED PROPERTIES DEFINITION
        private CavalryObjectKeyPlanTag _SelectedTag;
        public CavalryObjectKeyPlanTag SelectedTag
        {
            get {return _SelectedTag;}
            set { _SelectedTag = value; OnPropertyChanged(nameof(SelectedTag));}
        }


        private ObjectTag _SelectedObjectTag;
        public ObjectTag SelectedObjectTag
        {
            get { return _SelectedObjectTag; }
            set 
            { 
                _SelectedObjectTag = value; 
                SelectedTag = value.AvailableTagTypes.FirstOrDefault();
                OnPropertyChanged(nameof(SelectedObjectTag)); 
            }
        }

        private _ObservableCollection<string> _AvailableFilterKeys = new _ObservableCollection<string>();
        public _ObservableCollection<string> AvailableFilterKeys
        {
            get => _AvailableFilterKeys;
            set { _AvailableFilterKeys = value; OnPropertyChanged(nameof(AvailableFilterKeys)); }
        }

        private FilterModel _SelectedFilterType;
        public FilterModel SelectedFilterType
        {
            get { return _SelectedFilterType; }
            set
            {
                _SelectedFilterType = value;
                OnPropertyChanged(nameof(SelectedFilterType));
                AvailableFilterKeys.Reset(value.FilterKeys);
                SelectedFilterKey_ = AvailableFilterKeys.FirstOrDefault();
                SelectedFilterKeyIndex = 0;
            }
        }


        private int _SelectedFilterKeyIndex;
        public int SelectedFilterKeyIndex
        {
            get { return _SelectedFilterKeyIndex; }

            set
            {
                if (_SelectedFilterKeyIndex != value)
                {
                    _SelectedFilterKeyIndex = value;
                    OnPropertyChanged(nameof(SelectedFilterKeyIndex));
                }
            }
        }


        private string _SelectedFilterKey = "";
        public string SelectedFilterKey_
        {
            get 
            {
                return _SelectedFilterKey; 
            }
            set 
            {
                if (string.IsNullOrEmpty(value)) return;

                if (!_SelectedFilterKey.Equals(value))
                {
                    _SelectedFilterKey = value;
                    OnPropertyChanged(nameof(SelectedFilterKey_));
                }
            }
        }

        private bool _Multiple;
        public bool Multiple
        {
            get { return _Multiple; }
            set { _Multiple = value; OnPropertyChanged(nameof(Multiple));}
        }

        #endregion


        #region COMMAND DEFINITIONS
        private AddTagCommand _AddTagCommand;
        public AddTagCommand AddTagCommand => _AddTagCommand;
        #endregion


        #region SYSTEM FUNCTIONS
        public void ClosingWindow()
        {
            if (ClosedByXButton)
            {

            }
        }
        #endregion


        #region CONSTRUCTOR
        public TagMainViewModel()
        {
            try
            {
                InitializeModels();
                InitializeCommands();
                InitializeFirstObjects(); 
            }
            
            catch (Exception ex)
            {
                _Console.ShowConsole(ex.ToString());
            }
        }
        #endregion


        #region INITIALIZE FUNCTIONS
        private void InitializeModels()
        {
            _TagMainModel = new TagMainModel(this);
        }


        private void InitializeCommands()
        {
            _AddTagCommand = new AddTagCommand(this);
        }


        private void InitializeFirstObjects()
        {
            SelectedFilterType = _TagMainModel.FilterModelCollection.Filters[0];
            SelectedObjectTag = _TagMainModel.KeyPlanTagCollection.ObjectTagCollection[0];
            SelectedTag = _TagMainModel.KeyPlanTagCollection.CurrentAvailableTags[0];
            Multiple = false;
        }
        #endregion
    }
}
