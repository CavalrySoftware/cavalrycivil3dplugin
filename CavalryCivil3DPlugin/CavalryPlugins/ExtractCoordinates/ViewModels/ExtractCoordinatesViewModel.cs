using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.LayerManager;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Gis.Map.Topology;
using CavalryCivil3DPlugin.ACADLibrary.Selection;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.Commands;
using CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.Views;
using CavalryCivil3DPlugin.Consoles;
using CavalryCivil3DPlugin.Models;
using CavalryCivil3DPlugin.Models.CADObject;
using CavalryCivil3DPlugin.ACADLibrary._ObjectData;
using WindowsSelectionMode = System.Windows.Controls.SelectionMode;
using CavalryCivil3DPlugin.ACADLibrary.AutoCADTable;

namespace CavalryCivil3DPlugin.CavalryPlugins.ExtractCoordinates.ViewModel
{
    public class ExtractCoordinatesViewModel : INotifyPropertyChanged
    {

        #region << INITIALIZATION FOR INOTIFYPROPERYCHANGED >>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion


        #region << GLOBAL VARIABLES FOR DEPENDENCIES >>
        public readonly CivilDocument Civil3DDocument;
        public readonly Document AutocadDocument;
        public readonly Database AutocadDatabase;
        public readonly Editor _Editor;
        #endregion


        #region << MODEL PROPERTIES >>
        private CADObjectTypesMainModel CADObjectsMainModel_;
        public CADObjectTypesMainModel CADObjectsMainModel { get { return CADObjectsMainModel_; } set { OnPropertyChanged(nameof(CADObjectsMainModel));}}

        private TableStyleModel _TableStyles;
        public TableStyleModel TableStyles
        {
            get { return _TableStyles; }
            set { OnPropertyChanged(nameof(TableStyles));}
        }

        #endregion


        #region << VARIABLE PROPERTIES >>
        private string _PromptFilterKey;
        public string PromptFilterKey
        {
            get { return _PromptFilterKey; }
            set 
            {
                _PromptFilterKey = value;
                OnPropertyChanged(nameof(PromptFilterKey));
            }
        }

        private string _PromptSelectObjects;

        public string PromptSelectObjects
        {
            get { return _PromptSelectObjects; }
            set 
            {
                _PromptSelectObjects = value; 
                OnPropertyChanged(nameof(PromptSelectObjects));
            }
        }

        public string Prefix_ { get { return string.IsNullOrEmpty(_Prefix) ? "P" : _Prefix; } }

        private string _Prefix;
        public string Prefix
        {
            get { return _Prefix; }
            set 
            {
                _Prefix = value;
                OnPropertyChanged(nameof(Prefix));
            }
        }


        public List<string> PrefixList
        {
            get
            {
                return _Prefixes != null ? _Prefixes.Split(',').ToList().Select(x => x.Trim()).ToList() : new List<string>();
            }
        }


        private string _TableName;
        public string TableName
        {
            get { return _TableName; }
            set { _TableName = value; OnPropertyChanged(nameof(TableName));}
        }


        private string _Prefixes;
        public string Prefixes
        {
            get { return _Prefixes; }
            set 
            { 
                _Prefixes = value;
                OnPropertyChanged(nameof(Prefixes)); 
            }
        }


        private string _PrefixPlaceholder = " X, Y, Z";
        public string PrefixPlaceholder
        {
            get 
            {
                return _PrefixPlaceholder; 
            }
            set
            {
                _PrefixPlaceholder = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PrefixPlaceholder)));
            }
        }


        private string _PointGroupsName;
        public string PointGroupsName
        {
            get { return _PointGroupsName; }
            set { _PointGroupsName = value; OnPropertyChanged(nameof(PointGroupsName)); }
        }

        private string _XOffset = "0";
        public string XOffset
        {
            get { return _XOffset; }
            set { _XOffset = value; OnPropertyChanged(nameof(XOffset)); }
        }

        public double xOffset { get; set; } = 0;
        public double yOffset { get; set; } = 0;

        private string _YOffset = "0";
        public string YOffset
        {
            get { return _YOffset; }
            set 
            {
                _YOffset = value;
                OnPropertyChanged(nameof(YOffset)); 
            }
        }
        #endregion


        #region << SELECTED PROPERTIES >>
        private CADObjectModel SelectedObjectType_;
        public CADObjectModel SelectedObjectType
        {
            get { return SelectedObjectType_; }
            set
            {
                SelectedObjectType_ = value;
                SelectedFilter = SelectedObjectType.Filters.FirstOrDefault();
                SelectedFilterKey = SelectedFilter.FilterOptions.FirstOrDefault();
                UpdatePromptObjectSelection();
                OnPropertyChanged(nameof(SelectedObjectType));
            }
        }

        private string _SelectedFilterKey;

        public string SelectedFilterKey
        {
            get { return _SelectedFilterKey; }
            set 
            { 
                _SelectedFilterKey = value; 
                OnPropertyChanged(nameof(SelectedFilterKey));
            }
        }


        private dynamic selectedEntities_;
        public dynamic SelectedEntities
        {
            get { return selectedEntities_; }
            set
            {
                selectedEntities_ = value;
                OnPropertyChanged(nameof(SelectedEntities));
            }
        }


        private FilterModel_ _SelectedFilter;
        public FilterModel_ SelectedFilter
        {
            get { return _SelectedFilter; }
            set
            {
                _SelectedFilter = value;
                UpdateFilterSelectionMode(value.FilterName);
                UpdatePromptFilterKeySelection();
                SelectedFilterKey = SelectedFilter.FilterOptions.FirstOrDefault();
                OnPropertyChanged(nameof(SelectedFilter));
            }
        }


        private string _SelectedTableStyle;
        public string SelectedTableStyle
        {
            get { return _SelectedTableStyle; }
            set { _SelectedTableStyle = value; OnPropertyChanged(nameof(SelectedTableStyle));}
        }
        #endregion


        #region << BOOLEAN PROPERTIES >>
        private bool _IsCombined;
        public bool IsCombined
        {
            get { return _IsCombined; }
            set
            {
                if (_IsCombined != value)
                {
                    _IsCombined = value;
                    OnPropertyChanged(nameof(IsCombined));
                }
            }
        }


        private bool _IsAnnotate = true;
        public bool IsAnnotate
        {
            get { return _IsAnnotate; }
            set
            {
                if (_IsAnnotate != value)
                {
                    _IsAnnotate = value;
                    OnPropertyChanged(nameof(IsAnnotate));
                }
            }
        }

        private bool _IncludeElevation = false;
        public bool IncludeElevation
        {
            get { return _IncludeElevation; }
            set
            {
                if (_IncludeElevation != value)
                {
                    _IncludeElevation = value;
                    OnPropertyChanged(nameof(IncludeElevation));
                }
            }
        }


        private WindowsSelectionMode _FilterSelectionMode = WindowsSelectionMode.Extended;
        public WindowsSelectionMode FilterSelectionMode
        {
            get { return _FilterSelectionMode; }
            set
            {
                if (_FilterSelectionMode != value)
                {
                    _FilterSelectionMode = value;
                    OnPropertyChanged(nameof(FilterSelectionMode));
                }
            }
        }


        private bool _WillCreatePointGroups;
        public bool WillCreatePointGroups
        {
            get { return _WillCreatePointGroups; }
            set { _WillCreatePointGroups = value; OnPropertyChanged(nameof(WillCreatePointGroups)); }
        }
        #endregion


        #region << MONITORING GLOBAL VARIABLES AND FUNCTIONS >>
        public bool CanExtractCoordinates()
        {
            try
            {
                if (SelectedEntities.Count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            catch (Exception)
            {
                return false;
            }
        }

        public void OnTextBoxFocused()
        {
            PrefixPlaceholder = "";
        }

        public void OnTextBoxUnFocused()
        {
            PrefixPlaceholder = string.IsNullOrEmpty(_Prefixes) ? " X, Y, Z" : "";
        }


        public void OnXOffsetUnFocused()
        {
            if (!string.IsNullOrEmpty(_XOffset))
            {
                if(double.TryParse(_XOffset, out double result))
                {
                    xOffset = result;
                }
                else
                {
                    xOffset = 0;
                    XOffset = xOffset.ToString();
                }
            }
            else
            {
                xOffset = 0;
                XOffset = xOffset.ToString();
            }
        }


        public void OnYOffsetUnFocused()
        {
            if (!string.IsNullOrEmpty(_YOffset))
            {
                if (double.TryParse(_YOffset, out double result))
                {
                    yOffset = result;
                }
                else
                {
                    yOffset = 0;
                    YOffset = yOffset.ToString();
                }
            }
            else
            {
                yOffset = 0;
                YOffset = yOffset.ToString();
            }
        }
        #endregion


        #region << COMMAND INTERFACE >>
        private ExtractCoordinatesOk _ExtractCoordinatesBaseCommand;
        public ExtractCoordinatesOk ExtractCoordinatesBaseCommand { get { return _ExtractCoordinatesBaseCommand; } }

        private ICommand _ExtractCoordinatesCommand;
        public ICommand ExtractCoordinatesCommand
        {
            get { return _ExtractCoordinatesCommand; }
        }

        private ICommand _CancelCommand;

        public ICommand CancelCommand
        {
            get { return _CancelCommand; }
        }



        public Action CloseAction { get; set; }
        public Action HideAction { get; set; }


        
        private PickPolylines _PickPolylinesCommand;
        public PickPolylines PickPolylinesCommand
        {
            get { return _PickPolylinesCommand; }
        }
        #endregion


        #region << CONSTRUCTOR >>
        public ExtractCoordinatesViewModel()
        {
            Civil3DDocument = CivilApplication.ActiveDocument;
            AutocadDocument = Application.DocumentManager.MdiActiveDocument;
            AutocadDatabase = AutocadDocument.Database;
            _Editor = AutocadDocument.Editor;

            InitializeViewModel();
            InitializeCommands();
            //Test();
        }
        #endregion


        #region << INITIALIZATION FUNCTIONS >>
        private void InitializeViewModel()
        {
            List<string> cadObjects = new List<string>() {"Polyline", "Feature Lines"};

            CADObjectsMainModel_ = new CADObjectTypesMainModel(AutocadDocument, cadObjects, Civil3DDocument);
            SelectedObjectType = CADObjectsMainModel_.CADObjectTypes.FirstOrDefault();
            SelectedFilter = SelectedObjectType.Filters.FirstOrDefault();
            _TableStyles = new TableStyleModel(AutocadDocument);
            SelectedTableStyle = _TableStyles.ExistingStyleNames[_TableStyles.DefaultItem];

            TableName = "SETTING OUT POINTS";
        }


        public void InitializeCommands()
        {
            _ExtractCoordinatesBaseCommand = new ExtractCoordinatesOk(this);
            _ExtractCoordinatesCommand = new RelayCommand(_ExtractCoordinatesBaseCommand.CreateCoordinatesTableExecute, CanExtractCoordinates);
            _PickPolylinesCommand = new PickPolylines(this);
            _CancelCommand = new CancelCommand(this);
        }
        #endregion


        #region << FUNCTIONS >>
        public void UpdateAvailableObjects(object sender)
        {
            var listSelectedItems = ((ListBox)sender).SelectedItems;
            List<string> SelectedFilterKeys = listSelectedItems.Cast<string>().ToList();

            string filterName = _SelectedFilter.FilterName;

           
            if (filterName == "Layer")
            {
                try
                {
                    SelectedObjectType.LoadObjectsByLayer(SelectedFilterKeys);
                }
                
                catch (Exception ex) {_Console.ShowConsole(ex.ToString());}
            }

            else if (filterName.Contains("Object Data"))
            {
                string tableName = _SelectedFilter.FilterObject.TableName;
                string filterKey = SelectedFilterKeys.Count > 0 ? SelectedFilterKeys[0] : "";
                SelectedObjectType.LoadObjectsByObjectData(filterKey, tableName);
            }

            else if (filterName == "Site")
            {
                SelectedObjectType.LoadObjectsBySite(SelectedFilterKeys[0]);
            }
        }


        public void UpdateSelectedEntities(object sender)
        {
            var listSelectedItems = ((ListBox)sender).SelectedItems;

            switch (SelectedObjectType_.ObjectName)
            {
                case "Polyline":
                    SelectedEntities = listSelectedItems.Cast<PolylineModel>().ToList().OrderBy(x => x.Name).ToList();
                     break;
                case "Feature Lines":
                    SelectedEntities = listSelectedItems.Cast<FeatureLinesModel>().ToList().OrderBy(x => x.Name).ToList();
                    break;
            }
        }


        private void UpdatePromptObjectSelection()
        {
            string selectedObjectType = SelectedObjectType_.ObjectName;

            switch (selectedObjectType)
            {
                case "Polyline":
                    PromptSelectObjects = "Select Polylines:";
                    break;
                case "Feature Lines":
                    PromptSelectObjects = "Select Feature Lines:";
                    break;
            }
        }


        private void UpdateFilterSelectionMode(string filterType)
        {
            switch (filterType)
            {
                case "Layer":
                    FilterSelectionMode = WindowsSelectionMode.Extended;
                    break;
                default:
                    FilterSelectionMode = WindowsSelectionMode.Single;
                    break;
            }
        }


        private void UpdatePromptFilterKeySelection()
        {
            if (SelectedFilter.FilterName == "Site")
            {
                PromptFilterKey = "Select Site:";
            }

            else if (SelectedFilter.FilterName.Contains("Object Data"))
            {
                PromptFilterKey = "Select Property:";
            }

            else
            {
                PromptFilterKey = "Select Layer:";
            }
        }


        #endregion

        private void Test(string s = null)
        {
            
        }
    }
}
