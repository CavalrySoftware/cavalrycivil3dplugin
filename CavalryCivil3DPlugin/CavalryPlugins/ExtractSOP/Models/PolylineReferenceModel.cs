using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using CavalryCivil3DPlugin.ACADLibrary.Elements;
using CavalryCivil3DPlugin.ACADLibrary.Selection;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.ExtractSOP.Models
{
    public class PolylineReferenceModel
    {
        private ObjectId _Id;
        public ObjectId Id => _Id;

        private Document _AutocadDocument;
        private CivilDocument _C3DDocument;

        private bool _ValidPolylineReference;
        public bool ValidPolylineReference => _ValidPolylineReference;

        private List<Point3d> _PolylineCoordinates;
        public List<Point3d> PolylineCoordinates => _PolylineCoordinates;


        public PolylineReferenceModel(Document _autocadDocument, CivilDocument _civilDocument)
        {
            _AutocadDocument = _autocadDocument;
            _C3DDocument = _civilDocument;
        }


        public void SelectPolyline()
        {
            _Id = ACADObjectSelection.PickPolyline(_AutocadDocument);
            _ValidPolylineReference = _Id == ObjectId.Null ? false : true;
            SetPolylineReferenceProperties();
        }


        public void SetPolylineReferenceProperties()
        {
            if (_ValidPolylineReference) _PolylineCoordinates = _Polyline.GetAllVertices(_AutocadDocument, _Id);
        }
    }
}
