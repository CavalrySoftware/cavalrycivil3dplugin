using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Civil.ApplicationServices;
using CavalryCivil3DPlugin.ACADLibrary.Selection;

using AutocadEntity = Autodesk.AutoCAD.DatabaseServices.Entity;
using Civil3DEntity = Autodesk.Civil.DatabaseServices.Entity;
using ACADObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;
using System.Collections.ObjectModel;
using System.Windows.Automation;
using CavalryCivil3DPlugin.Consoles;
using System.Net.Http.Headers;
using CavalryCivil3DPlugin.C3DLibrary.Selection;
using Autodesk.Aec.Geometry;
using CavalryCivil3DPlugin.ACADLibrary._ObjectData;



namespace CavalryCivil3DPlugin.Models.CADObject
{
    public class CADObjectModel
    {

        #region << CAD DEPENDENCIES >>
        public readonly CivilDocument Civil3DDocument;
        public readonly Document AutocadDocument;
        public readonly Database AutocadDatabase;
        public readonly Editor _Editor;
        #endregion


        #region << PROPERTIES >>
        private string ObjectName_;
        public string ObjectName { get { return ObjectName_; } set { ObjectName_ = value; }}


        private string _ApplicationType;
        public string ApplicationType { get { return _ApplicationType; } }

        private List<PolylineModel> _PolylineModels;
        public List<PolylineModel> PolylineModels { get { return _PolylineModels;} }
        public ObservableCollection<PolylineModel> PolylineModelsCollection;


        private List<FeatureLinesModel> _FeatureLinesModel;
        public List<FeatureLinesModel> FeatureLinesModel { get { return _FeatureLinesModel; } }
        public ObservableCollection<FeatureLinesModel> FeatureLinesModelCollection;


        public dynamic AllExistingEntitiesCollection
        {
            get
            {
                switch (ObjectName_)
                {
                    case "Polyline":
                        return PolylineModelsCollection;

                    case "Feature Lines":
                        return FeatureLinesModelCollection;

                    default:
                        throw new System.Exception("Object Type not available.");
                }
            }
        }


        public dynamic AllExistingEntities
        {
            get
            {
                switch (ObjectName_)
                {
                    case "Polyline":
                        return _PolylineModels;
                    default:
                        return null;
                }
            }
        }

        private List<string> Layers_;

        public List<string> Layers
        {
            get { return Layers_; }
            set { Layers_ = value; }
        }


        private List<FilterModel> _Filters = new List<FilterModel>();
        public List<FilterModel> Filters
        {
            get { return _Filters;}
        }


        private List<ObjectDataModel> _ObjectDataModel = new List<ObjectDataModel>();
        #endregion



        #region << INTERNAL VARIABLES >>
        private List<string> ACADObjectNames = new List<string>() { "Polyline" };
        private List<string> C3DObjectNames = new List<string>() {"Feature Lines"};
        #endregion



        #region << CONSTRUCTOR >>
        public CADObjectModel(string _objectName, Document _document, CivilDocument _civilDocument)
        {
            ObjectName_ = _objectName;
            AutocadDocument = _document;
            Civil3DDocument = _civilDocument;
            _ApplicationType = ACADObjectNames.Contains(_objectName) ? "AutoCAD" : "Civil3D";

            InitializeModel();

        }
        #endregion



        #region << LOAD POLYLINES FUNCTIONS >>
        private void LoadPolylines()
        {
            List<ACADObjectId> polylineIds = ACADObjectSelection.GetAllPolylineId(AutocadDocument);

            int index = 1;
            _PolylineModels = polylineIds.Select(x => new PolylineModel(x, AutocadDocument, index++)).ToList();
        }


        private void LoadPolyLinesByLayer(List<string> _layers)
        {
            List<ACADObjectId> polylineIds = ACADObjectSelection.GetAllPolylineIdByLayers(AutocadDocument, _layers);

            int index = 1;
            _PolylineModels = polylineIds.Select(x => new PolylineModel(x, AutocadDocument, index)).ToList().OrderBy(x => x.LayerName).ToList();

            foreach(var plineModel in  _PolylineModels)
            {
                plineModel.Index = index++;
            }

            UpdateCollection();
        }


        private void LoadPolyLinesByObjectData(string _filterKey, string _tableName)
        {

            _PolylineModels = new List<PolylineModel>();

            if (!string.IsNullOrEmpty(_filterKey))
            {
                List<(ACADObjectId, string)> polylineIds = ACADObjectSelection.GetAllPolylineIdByObjectData(AutocadDocument, _tableName, _filterKey);
                int index = 1;
                foreach (var IdString in polylineIds)
                {
                    _PolylineModels.Add(new PolylineModel(IdString.Item1, AutocadDocument, index++, IdString.Item2));
                }
            }
            UpdateCollection();
        }
        #endregion



        #region << LOAD FEATURE LINES FUNCTIONS >>
        private void LoadAllFeatureLines()
        {
            List<ACADObjectId> featureLinesIds = C3DObjectSelection.GetAllFeatureLineIds(Civil3DDocument, AutocadDocument);
            LoadFeatureLines(featureLinesIds);
        }


        private void LoadFeatureLinesByLayer(List<string> _layers)
        {
            List<ACADObjectId> featureLinesIds = C3DObjectSelection.GetAllFeatureLineIdsByLayer(AutocadDocument, _layers);
            LoadFeatureLines(featureLinesIds);
        }


        private void LoadFeatureLinesBySite(string  _site)
        {
            List<ACADObjectId> featureLinesIds = C3DObjectSelection.GetAllFeatureLineIdsBySiteName(AutocadDocument, Civil3DDocument, _site);
            LoadFeatureLines(featureLinesIds);
        }


        private void LoadFeatureLines(List<ACADObjectId> _featureLineIds)
        {
            int index = 1;
            _FeatureLinesModel = _featureLineIds.Select(x => new FeatureLinesModel(x, AutocadDocument, index++)).ToList();
            UpdateCollection();
        }
        #endregion



        #region << GENERAL LOAD OBJECTS FUNCTIONS >>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_layers"></param>
        public void LoadObjectsByLayer(List<string> _layers)
        {
            
            if (ObjectName_ == "Polyline")
            {
                LoadPolyLinesByLayer(_layers);
            }

            else if (ObjectName_ == "Feature Lines")
            {
                LoadFeatureLinesByLayer(_layers);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="_filterKey"></param>
        /// <param name="_tableName"></param>
        public void LoadObjectsByObjectData(string _filterKey, string _tableName)
        {
            if (ObjectName_ == "Polyline")
            {
                LoadPolyLinesByObjectData(_filterKey, _tableName);
            }
        }


        public void LoadObjectsBySite(string _filterKey)
        {
            if (ObjectName_ == "Feature Lines")
            {
                LoadFeatureLinesBySite(_filterKey);
            }
        }
        #endregion


        #region << UPDATE FUNCTIONS >>
        private void UpdateCollection()
        {
            if (ObjectName_ == "Polyline")
            {
                PolylineModelsCollection.Clear();

                foreach (var plineModel in _PolylineModels)
                {
                    PolylineModelsCollection.Add(plineModel);
                }
            }

            else if (ObjectName_ == "Feature Lines")
            {
                FeatureLinesModelCollection.Clear();
                foreach (var featureLineModel in _FeatureLinesModel)
                {
                    FeatureLinesModelCollection.Add(featureLineModel);
                }
            }
        }
        #endregion


        #region << INITIALIZATION FUNCTIONS >>
        private void InitializeModel()
        {
            if(ObjectName_ == "Polyline")
            {
                PolylineModelsCollection = new ObservableCollection<PolylineModel>();
            }

            else if (ObjectName_ == "Feature Lines")
            {
                FeatureLinesModelCollection = new ObservableCollection<FeatureLinesModel>();
            }

            else
            {
                throw new System.Exception("Object Type is not available.");
            }

            SetFilters();
        }


        private void SetFilters()
        {
            switch (ObjectName_)
            {
                case "Polyline":
                    _Filters.Add(new FilterModel(new LayersModel(AutocadDocument)));

                    Dictionary<string, List<string>> fields = _ObjectDataTable.GetAllFields(AutocadDocument);
                    foreach (var field in fields.Keys)
                    {
                        _ObjectDataModel.Add(new ObjectDataModel(field, fields[field]));
                    }

                    foreach (var objectDataModel in _ObjectDataModel)
                    {
                        FilterModel filter = new FilterModel(objectDataModel);
                        _Filters.Add(filter);
                    }
                    break;

                case "Feature Lines":
                    _Filters.Add(new FilterModel(new SitesModel(AutocadDocument, Civil3DDocument)));
                    _Filters.Add(new FilterModel(new LayersModel(AutocadDocument)));
                    break;  
            }
        }
        #endregion


    }
}
