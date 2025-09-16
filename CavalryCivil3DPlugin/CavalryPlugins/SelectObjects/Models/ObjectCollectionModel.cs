using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using CavalryCivil3DPlugin.ACADLibrary.Selection;
using CavalryCivil3DPlugin.C3DLibrary.Selection;

namespace CavalryCivil3DPlugin.CavalryPlugins.SelectObjects.Models
{
    public class ObjectCollectionModel
    {
        private List<ObjectModel> _ObjectModels = new List<ObjectModel>();
        public List<ObjectModel> ObjectModels => _ObjectModels;

        private Document _AutocadDocument;
        private CivilDocument _CivilDocument;

        public ObjectCollectionModel(Document _autocadDocument, CivilDocument _civilDocument)
        {
            _AutocadDocument = _autocadDocument;
            _CivilDocument = _civilDocument;

            ObjectModel alignmentModel = new ObjectModel(typeof(Alignment));
            ObjectModel polylineModel = new ObjectModel(typeof(Polyline));
            ObjectModel pointModel = new ObjectModel(typeof(DBPoint));

            _ObjectModels.Add(alignmentModel);
            _ObjectModels.Add(polylineModel); 
            _ObjectModels.Add(pointModel);
        }

        private List<ObjectElementModel> _Elements;
        public List<ObjectElementModel> Elements => _Elements;


        public void GetObjects(Type _objectType, string _layer, string _propType, string _propField)
        {
            List<ObjectId> objectIds = new List<ObjectId>();

            if (_objectType == typeof(Alignment))
            {
                objectIds = _layer == "No Filter" ? C3DObjectSelection.GetAllAlignments(_AutocadDocument, _CivilDocument) : C3DObjectSelection.GetAlignmentIdsByLayer(_AutocadDocument, _CivilDocument, new List<string>() { _layer});
            }


            else if (_objectType == typeof(Polyline))
            {
                objectIds = _layer == "No Filter" ? ACADObjectSelection.GetAllPolylineId(_AutocadDocument) : ACADObjectSelection.GetAllPolylineIdByLayers(_AutocadDocument, new List<string>() { _layer });
            }


            else if (_objectType == typeof(DBPoint))
            {
                objectIds = _layer == "No Filter" ? C3DObjectSelection.GetAllPoints(_AutocadDocument, _CivilDocument) : C3DObjectSelection.GetAllPointsByLayer(_AutocadDocument, _CivilDocument, new List<string>() { _layer });
            }

            List<ObjectElementModel> objectElements = new List<ObjectElementModel>();
            foreach (ObjectId objectId in objectIds)
            {
                ObjectElementModel objectElementModel = new ObjectElementModel()
                {
                    ObjectId = objectId
                };

                objectElements.Add(objectElementModel);
            }

            _Elements = objectElements;
        }
    }
}
