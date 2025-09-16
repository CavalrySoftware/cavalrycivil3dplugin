using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CavalryCivil3DPlugin.CavalryPlugins.SelectObjects.Models;
using CavalryCivil3DPlugin.Consoles;
using Newtonsoft.Json.Bson;

namespace CavalryCivil3DPlugin.CavalryPlugins.SelectObjects.ViewModels
{
    public class SelectObjectsMainViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));


        #region MODEL PROPERTIES
        private SelectObjectsMainModel _SelectObjectsMainModel;
        public SelectObjectsMainModel SelectObjectsMainModel => _SelectObjectsMainModel;
        #endregion


        #region CONSTRUCTOR
        public SelectObjectsMainViewModel()
        {
            InitializeModels();
            InitializeSelectedObjects();
        }
        #endregion


        #region << WINDOW ACTIONS PROPERTIES >>
        public Action CloseAction { get; set; }
        public Action HideAction { get; set; }
        public Action ShowAction { get; set; }
        public bool ClosedByXButton = false;

        public void ClosingWindow()
        {
            if (ClosedByXButton)
            {

            }
        }
        #endregion


        #region BINDING PROPERTIES
        private ObjectModel _SelectedObjectModel;
        public ObjectModel SelectedObjectModel
        {
            get { return _SelectedObjectModel; }
            set
            {
                _SelectedObjectModel = value;
                OnPropertyChanged(nameof(SelectedObjectModel));
                if (SelectedPropertyType != null) RefreshObjects();
            }
        }


        private PropertyTypeModel _SelectedPropertyType;
        public PropertyTypeModel SelectedPropertyType
        {
            get { return _SelectedPropertyType; }
            set 
            { 
                _SelectedPropertyType = value; 
                OnPropertyChanged(nameof(SelectedPropertyType));
                SelectedProperty = _SelectedPropertyType.Properties.FirstOrDefault();
            }
        }


        private string _SelectedLayer;
        public string SelectedLayer
        {
            get { return _SelectedLayer; }
            set 
            { 
                _SelectedLayer = value; 
                OnPropertyChanged(nameof(SelectedLayer));
                if (SelectedPropertyType != null) RefreshObjects();
            }
        }


        private PropertyModel _SelectedProperty;
        public PropertyModel SelectedProperty
        {
            get { return _SelectedProperty; }
            set 
            {
                _SelectedProperty = value;
                OnPropertyChanged(nameof(SelectedProperty));
                SelectedPropertyField = value.Fields.FirstOrDefault();
            }
        }


        private string _SelectedPropertyField;
        public string SelectedPropertyField
        {
            get { return _SelectedPropertyField; }
            set { _SelectedPropertyField = value; OnPropertyChanged(nameof(SelectedPropertyField));}
        }

        private int _TotalObjects;
        public int TotalObjects
        {
            get { return _TotalObjects; }
            set { _TotalObjects = value; OnPropertyChanged(nameof(TotalObjects));}
        }
        #endregion


        #region
        private void InitializeModels()
        {
            _SelectObjectsMainModel = new SelectObjectsMainModel();
        }
        #endregion


        #region
        private void InitializeSelectedObjects()
        {
            SelectedObjectModel = _SelectObjectsMainModel.ObjectCollectionModel.ObjectModels.FirstOrDefault();
            SelectedPropertyType = _SelectObjectsMainModel.PropertyCollection.PropertyTypeModels.FirstOrDefault();
            SelectedLayer = _SelectObjectsMainModel.LayerCollection.Layers.FirstOrDefault();

            RefreshObjects();
        }


        private void RefreshObjects()
        {
            try
            {
                _SelectObjectsMainModel.ObjectCollectionModel.GetObjects(SelectedObjectModel.ObjectType, SelectedLayer, SelectedPropertyType.Name, SelectedPropertyField);
                TotalObjects = _SelectObjectsMainModel.ObjectCollectionModel.Elements.Count();
            }

            catch (Exception ex) {_Console.ShowConsole(ex.ToString());} 

           
            
        }
        #endregion



    }
}
