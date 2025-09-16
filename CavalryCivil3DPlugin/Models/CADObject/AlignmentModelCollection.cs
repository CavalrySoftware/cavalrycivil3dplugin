using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;

namespace CavalryCivil3DPlugin.Models.CADObject
{
    public class AlignmentModelCollection
    {
        private List<ObjectId> _AlignmentIds;
        public List<ObjectId> AlignmentIds => _AlignmentIds;

        private Document _AutocadDocument;
        private CivilDocument _C3DDocument;

        public AlignmentModelCollection(Document _autocadDocument, CivilDocument _civilDocument)
        {
            _AutocadDocument = _autocadDocument;
            _C3DDocument = _civilDocument;
        }


        public void SetCollectionByLayer(string _layerName)
        {

        }



    }
}
