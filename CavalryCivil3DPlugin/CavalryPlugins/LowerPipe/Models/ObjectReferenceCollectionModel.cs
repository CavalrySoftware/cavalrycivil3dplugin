using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Models
{
    public class ObjectReferenceCollectionModel
    {

        private Document _AutocadDocument;

        private List<ObjectReferenceModel> _ObjectReferences = new List<ObjectReferenceModel>();
        public List<ObjectReferenceModel> ObjectReferences { get { return _ObjectReferences; } }

        public ObjectReferenceModel _ActiveReference { get; set; }

        public ObjectReferenceCollectionModel(Document _autocadDocument)
        {
            _AutocadDocument = _autocadDocument; 
            InitializeReferences();
        }


        private List<string> _ReferenceNames = new List<string>()
        {
            "Pressure Pipe",
            "Line",
            "Polyline",
        };


        private void InitializeReferences()
        {
            foreach (var reference in _ReferenceNames)
            {
                _ObjectReferences.Add(new ObjectReferenceModel(reference, _AutocadDocument));
            }
        }

    }
}
