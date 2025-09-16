using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using CavalryCivil3DPlugin._Library._ACADLibrary.Annotation;
using CavalryCivil3DPlugin._Library._C3DLibrary._PropertySet;
using CavalryCivil3DPlugin._Library._C3DLibrary.Elements;
using CavalryCivil3DPlugin.ACADLibrary._ObjectData;
using CavalryCivil3DPlugin.ACADLibrary.Elements;
using CavalryCivil3DPlugin.ACADLibrary.Selection;
using CavalryCivil3DPlugin.C3DLibrary.Selection;
using CavalryCivil3DPlugin.CavalryPlugins.Tag.ViewModels;
using CavalryCivil3DPlugin.Consoles;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using CEntity = Autodesk.Civil.DatabaseServices.Entity;
using CPoint = Autodesk.Civil.DatabaseServices.Point;

namespace CavalryCivil3DPlugin.CavalryPlugins.Tag.Models
{
    public class CavalryObjectKeyPlanTag
    {

        #region CAD DEPENDENCIES
        private Document _AutocadDocument;
        public CivilDocument _CivilDocument;
        #endregion


        #region OBJECT PROPERTIES
        private Type _ObjectType;
        private Point3d _PickedPoint;
        private Point3d _InsertionPoint;

        private ObjectId _ObjectId;
        public ObjectId ObjectId => _ObjectId;

        private List<ObjectId> _MultipleObjectIds;

        private string _Name;
        public string Name => _Name;

        private string _TagText;
        public string TagText => _TagText;

        private string  _PropertySet;
        public string  PropertySet => _PropertySet;

        private List<string> _Fields;
        public List<string> Fields => _Fields;

        private string _KeyFilter;
        private string _ValueFilter;

        private string _ObjectPropName;
        public string ObjectPropName => _ObjectPropName;
        #endregion


        public CavalryObjectKeyPlanTag(Document _autocadDocument, CivilDocument _civilDocument, Type _objectType, string _name, string _propertySet, List<string> _fields, string keyFilter = null, string valueFilter = null)
        {
            _AutocadDocument = _autocadDocument;
            _CivilDocument = _civilDocument;

            _ObjectType = _objectType;
            _Name = _name;
            _PropertySet = _propertySet;
            _Fields = _fields;
            _KeyFilter = keyFilter;
            _ValueFilter = valueFilter;
        }

        public CavalryObjectKeyPlanTag(Document _autocadDocument, CivilDocument _civilDocument, string _name, Type _objectType, string _objectProp, List<string> _fields)
        {
            _AutocadDocument = _autocadDocument;
            _CivilDocument = _civilDocument;

            _ObjectType = _objectType;
            _ObjectPropName = _objectProp;
            _Name = _name;
            _Fields = _fields;
        }


        public void TagObjectExecute(TagMainViewModel.NotificationMethod _selectionOption, string _filterType=null, string _filterKey=null)
        {
            if (_selectionOption == TagMainViewModel.NotificationMethod.Single) TagSingleObjects();
            else TagMultiple(_selectionOption, _filterType, _filterKey);
        }


        private void PickObject()
        {
            if (_ObjectType == typeof(Alignment))
            {
                (_ObjectId, _PickedPoint) = C3DObjectSelection.PickAlignmentWithPoint(_AutocadDocument);
            }
                
            else if (_ObjectType == typeof(PressurePart))
            {
                (_ObjectId, _PickedPoint) = C3DObjectSelection.PickPressureComponentsWithPoint(_AutocadDocument);
            }

            else if (_ObjectType == typeof(CPoint))
            {
                (_ObjectId, _PickedPoint) = C3DObjectSelection.PickPointWithPoint(_AutocadDocument);
            }

            else if (_ObjectType == typeof(CogoPoint))
            {
                (_ObjectId, _PickedPoint) = C3DObjectSelection.PickCogoPointWithPoint(_AutocadDocument);
            }

            else if (_ObjectType == typeof(Polyline))
            {
                (_ObjectId, _PickedPoint) = C3DObjectSelection.PickPolylineWithPoint(_AutocadDocument);
            }
        }


        private void TagSingleObjects()
        {
            while (true)
            {
                PickObject();

                if (_ObjectId == ObjectId.Null) break;

                bool isObjectData = (_ObjectType == typeof(CPoint));
                bool isObjectName = (_ObjectPropName == "Name");
                GetPropertiesFromObject(isObjectData, _objectPropName: isObjectName);
                _MLeader.CreateMLead(_AutocadDocument, _TagText, _PickedPoint);
            }
        }


        public void TagMultiple(TagMainViewModel.NotificationMethod _selectionOption, string _filterType = null, string _filterKey = null)
        {
            if (_ObjectType == typeof(Alignment))
            {
                TagMultipleAlignments(_selectionOption, _filterType, _filterKey);
            }

            else if (_ObjectType == typeof(PressurePart))
            {
                TagPressureComponentObjects(_selectionOption, _filterType, _filterKey);
            }

            else if ( _ObjectType == typeof(CPoint))
            {
                TagPointObjects(_selectionOption, _filterType, _filterKey);
            }

            else if (_ObjectType == typeof(CogoPoint))
            {
                TagCOGOPointObjects(_selectionOption, _filterType, _filterKey);
            }

            else if (_ObjectType == typeof(Polyline))
            {
                TagMultiplePolylines(_selectionOption, _filterType, _filterKey);
            }

        }


        #region ALIGNMENT OBJECTS
        private void TagMultipleAlignments(TagMainViewModel.NotificationMethod _selectionOption, string _filterType = null, string _filterKey = null)
        {
            using (Transaction tr = _AutocadDocument.Database.TransactionManager.StartTransaction())
            {
                SetAlignmentObjects(_selectionOption, _filterType, _filterKey);
                foreach (ObjectId alignmentId in _MultipleObjectIds)
                {
                    _ObjectId = alignmentId;
                    _PickedPoint = _Alignment.GetMidPoint(_AutocadDocument, _ObjectId);
                    GetPropertiesFromObject();
                    _MLeader.CreateMLead(_AutocadDocument, _TagText, _PickedPoint);
                }

                tr.Commit();
            }
            
        }


        private void SetAlignmentObjects(TagMainViewModel.NotificationMethod _selectionMethod, string _filterType, string _filterKey)
        {
            List<ObjectId> alignmentIds = new List<ObjectId>();

            switch (_selectionMethod)
            {

                case TagMainViewModel.NotificationMethod.Multiple:
                    switch (_filterType)
                    {
                        case "None":
                            alignmentIds = C3DObjectSelection.PickAlignments(_AutocadDocument, _CivilDocument);
                            break;

                        case "Layer":
                            List<string> layerList = new List<string>() { _filterKey };
                            alignmentIds = C3DObjectSelection.PickAlignmentsByLayer(_AutocadDocument, _CivilDocument, layerList);
                            break;
                    }
                    break;
                case TagMainViewModel.NotificationMethod.All:
                    switch (_filterType)
                    {
                        case "None":
                            alignmentIds = C3DObjectSelection.GetAllAlignments(_AutocadDocument, _CivilDocument);
                            break;

                        case "Layer":
                            List<string> layerList = new List<string>() { _filterKey };
                            alignmentIds = C3DObjectSelection.GetAlignmentIdsByLayer(_AutocadDocument, _CivilDocument, layerList);
                            break;
                    }
                    break;
            }

            _MultipleObjectIds = alignmentIds;
        }
        #endregion


        #region POLYLINE
        private void TagMultiplePolylines(TagMainViewModel.NotificationMethod _selectionOption, string _filterType = null, string _filterKey = null)
        {
            using (Transaction tr = _AutocadDocument.Database.TransactionManager.StartTransaction())
            {
                SetPolylineObjects(_selectionOption, _filterType, _filterKey);
                foreach (ObjectId polylineId in _MultipleObjectIds)
                {
                    _ObjectId = polylineId;
                    _PickedPoint = _Polyline.GetMidPoint(_AutocadDocument, _ObjectId);
                    GetPropertiesFromObject();
                    _MLeader.CreateMLead(_AutocadDocument, _TagText, _PickedPoint);
                }

                tr.Commit();
            }

        }


        private void SetPolylineObjects(TagMainViewModel.NotificationMethod _selectionMethod, string _filterType, string _filterKey)
        {
            List<ObjectId> polylineIds = new List<ObjectId>();

            switch (_selectionMethod)
            {

                case TagMainViewModel.NotificationMethod.Multiple:
                    switch (_filterType)
                    {
                        case "None":
                            polylineIds = ACADObjectSelection.PickPolylines(_AutocadDocument);
                            break;

                        case "Layer":
                            List<string> layerList = new List<string>() { _filterKey };
                            polylineIds = ACADObjectSelection.PickPolylinesByLayer(_AutocadDocument, layerList);
                            break;
                    }
                    break;
                case TagMainViewModel.NotificationMethod.All:
                    switch (_filterType)
                    {
                        case "None":
                            polylineIds = ACADObjectSelection.GetAllPolylineId(_AutocadDocument);
                            break;

                        case "Layer":
                            List<string> layerList = new List<string>() { _filterKey };
                            polylineIds = ACADObjectSelection.GetAllPolylineIdByLayers(_AutocadDocument, layerList);
                            break;
                    }
                    break;
            }

            _MultipleObjectIds = polylineIds;
        }
        #endregion


        #region PRESSURE COMPONENTS

        private void TagPressureComponentObjects(TagMainViewModel.NotificationMethod _selectionOption, string _filterType = null, string _filterKey = null)
        {
            using (Transaction tr = _AutocadDocument.Database.TransactionManager.StartTransaction())
            {
                SetPressureComponentObjects(_selectionOption, _filterType, _filterKey);
                foreach (ObjectId componentId in _MultipleObjectIds)
                {
                    _ObjectId = componentId;
                    CEntity entity = tr.GetObject(componentId, OpenMode.ForRead) as CEntity;
                    _PickedPoint = entity.StartPoint;
                    GetPropertiesFromObject();
                    _MLeader.CreateMLead(_AutocadDocument, _TagText, _PickedPoint);
                }

                tr.Commit();
            }

        }

        private void SetPressureComponentObjects(TagMainViewModel.NotificationMethod _selectionMethod, string _filterType, string _filterKey)
        {
            List<ObjectId> pressureComponentIds = new List<ObjectId>();

            switch (_selectionMethod)
            {

                case TagMainViewModel.NotificationMethod.Multiple:
                    switch (_filterType)
                    {
                        case "None":
                            pressureComponentIds = C3DObjectSelection.PickPressureComponents(_AutocadDocument, _CivilDocument);
                            break;

                        case "Layer":
                            List<string> layerList = new List<string>() { _filterKey };
                            pressureComponentIds = C3DObjectSelection.PickPressureComponentsByLayer(_AutocadDocument, _CivilDocument, layerList);
                            break;
                    }
                    break;
                case TagMainViewModel.NotificationMethod.All:
                    switch (_filterType)
                    {
                        case "None":
                            pressureComponentIds = C3DObjectSelection.GetAllFittingsAndAppurtenances(_AutocadDocument, _CivilDocument);
                            break;

                        case "Layer":
                            List<string> layerList = new List<string>() { _filterKey };
                            pressureComponentIds = C3DObjectSelection.GetFittingsAndAppurtenancesByLayer(_AutocadDocument, _CivilDocument, layerList);
                            break;
                    }
                    break;
            }

            _MultipleObjectIds = pressureComponentIds;
        }
        #endregion


        #region POINT
        private void TagPointObjects(TagMainViewModel.NotificationMethod _selectionOption, string _filterType = null, string _filterKey = null)
        {
            using (Transaction tr = _AutocadDocument.Database.TransactionManager.StartTransaction())
            {
                SetPointObjects(_selectionOption, _filterType, _filterKey);
                foreach (ObjectId componentId in _MultipleObjectIds)
                {
                    _ObjectId = componentId;
                    DBPoint point = tr.GetObject(componentId, OpenMode.ForRead) as DBPoint;
                    _PickedPoint = point.Position;
                    GetPropertiesFromObject(_OD: true);
                    _MLeader.CreateMLead(_AutocadDocument, _TagText, _PickedPoint);
                }

                tr.Commit();
            }
        }

        private void SetPointObjects(TagMainViewModel.NotificationMethod _selectionMethod, string _filterType, string _filterKey)
        {
            List<ObjectId> pointIds = new List<ObjectId>();

            switch (_selectionMethod)
            {

                case TagMainViewModel.NotificationMethod.Multiple:
                    switch (_filterType)
                    {
                        case "None":
                            pointIds = C3DObjectSelection.PickPoints(_AutocadDocument, _CivilDocument);
                            break;

                        case "Layer":
                            List<string> layerList = new List<string>() { _filterKey };
                            pointIds = C3DObjectSelection.PickPointsByLayer(_AutocadDocument, _CivilDocument, layerList);
                            break;
                    }
                    break;

                case TagMainViewModel.NotificationMethod.All:
                    switch (_filterType)
                    {
                        case "None":
                            pointIds = C3DObjectSelection.GetAllPoints(_AutocadDocument, _CivilDocument);
                            break;

                        case "Layer":
                            List<string> layerList = new List<string>() { _filterKey };
                            pointIds = C3DObjectSelection.GetAllPointsByLayer(_AutocadDocument, _CivilDocument, layerList);
                            break;
                    }
                    break;
            }

            _MultipleObjectIds = _KeyFilter != null && _ValueFilter != null ? FilterObjectsOD(pointIds) : pointIds;
            
        }
        #endregion


        #region COGO POINT
        private void TagCOGOPointObjects(TagMainViewModel.NotificationMethod _selectionOption, string _filterType = null, string _filterKey = null)
        {
            using (Transaction tr = _AutocadDocument.Database.TransactionManager.StartTransaction())
            {
                SetCOGOPointObjects(_selectionOption, _filterType, _filterKey);
                foreach (ObjectId componentId in _MultipleObjectIds)
                {
                    _ObjectId = componentId;
                    CogoPoint point = tr.GetObject(componentId, OpenMode.ForRead) as CogoPoint;
                    _PickedPoint = point.Location;
                    bool isObjectName = (_ObjectPropName == "Name");
                    GetPropertiesFromObject(_objectPropName: isObjectName);
                    _MLeader.CreateMLead(_AutocadDocument, _TagText, _PickedPoint);
                }

                tr.Commit();
            }
        }

        private void SetCOGOPointObjects(TagMainViewModel.NotificationMethod _selectionMethod, string _filterType, string _filterKey)
        {
            List<ObjectId> pointIds = new List<ObjectId>();

            switch (_selectionMethod)
            {

                case TagMainViewModel.NotificationMethod.Multiple:
                    switch (_filterType)
                    {
                        case "None":
                            pointIds = C3DObjectSelection.PickCOGOPoints(_AutocadDocument, _CivilDocument);
                            break;

                        case "Layer":
                            List<string> layerList = new List<string>() { _filterKey };
                            pointIds = C3DObjectSelection.PickCogoPointsByLayer(_AutocadDocument, _CivilDocument, layerList);
                            break;
                    }
                    break;

                case TagMainViewModel.NotificationMethod.All:
                    switch (_filterType)
                    {
                        case "None":
                            pointIds = C3DObjectSelection.GetAllCOGOPoints(_AutocadDocument, _CivilDocument);
                            break;

                        case "Layer":
                            List<string> layerList = new List<string>() { _filterKey };
                            pointIds = C3DObjectSelection.GetAllCOGOPointsByLayer(_AutocadDocument, _CivilDocument, layerList);
                            break;
                    }
                    break;
            }

            _MultipleObjectIds = _KeyFilter != null && _ValueFilter != null ? FilterObjectsOD(pointIds) : pointIds;

        }
        #endregion


        private List<ObjectId> FilterObjectsOD(List<ObjectId> _objectIds)
        {
            List<ObjectId> filteredIds = new List<ObjectId>();
            foreach(ObjectId objectId in _objectIds)
            {
                Dictionary<string, string> properties = _ObjectDataTable.GetPropFromObject(_AutocadDocument, objectId, _PropertySet);

                if (properties.Keys.Contains(_KeyFilter))
                {
                    if (properties[_KeyFilter] == _ValueFilter)
                    {
                        filteredIds.Add(objectId);
                    }
                }   
            }

            return filteredIds; 
        }


        private void GetPropertiesFromObject(bool _OD = false, bool _objectPropName = false)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            if (_OD)
            {
                properties = _ObjectDataTable.GetPropFromObject(_AutocadDocument, _ObjectId, _PropertySet);
            }

            else if (_objectPropName && _ObjectType == typeof(CogoPoint))
            {
                properties["Name"] = _COGOPoint.GetName(_AutocadDocument, _ObjectId);
            }

            else
            {
                properties = C3DPropertySet.GetObjectPropertySet(_ObjectId, _PropertySet, _AutocadDocument);
            }

            _TagText = "";
            string propValue = "";

            using (Transaction tr = _AutocadDocument.Database.TransactionManager.StartTransaction())
            {
                int index = 0;

                foreach (string field in Fields)
                {
                    string actualField = field;
                    string prefix = "";

                    if (field.Contains("%"))
                    {
                        prefix = field.Split('%')[0];
                        actualField = field.Split('%')[1];
                    }

                    if (field == "<Length>")
                    {
                        Alignment alignment = tr.GetObject(_ObjectId, OpenMode.ForRead) as Alignment;
                        propValue = $"{alignment.Length:0.00}m".Trim();
                    }

                    if (properties.ContainsKey(actualField))
                    {
                        propValue = properties[actualField].Trim().ToUpper();
                    }

                    if (index == 0)
                    {
                        _TagText = $"{prefix} {propValue}".Trim().ToUpper();
                        index++;
                    }

                    else
                    {
                        _TagText = string.IsNullOrEmpty(propValue) || string.IsNullOrWhiteSpace(propValue) ? _TagText : _TagText + "\n" + $"{prefix} {propValue}".Trim();
                    }
                }
            }
            
        }
    }
}
