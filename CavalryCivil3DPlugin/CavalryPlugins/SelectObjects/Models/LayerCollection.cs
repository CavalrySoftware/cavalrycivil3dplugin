using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using CavalryCivil3DPlugin.ACADLibrary.Selection;

namespace CavalryCivil3DPlugin.CavalryPlugins.SelectObjects.Models
{
    public class LayerCollection
    {

        private List<string> _Layers;
        public List<string> Layers => _Layers;

        private Document _AutocadDocument;

        public LayerCollection(Document _autocadDocument)
        {
            _AutocadDocument = _autocadDocument;
            _Layers = ACADObjectSelection.GetAllLayerNames(_AutocadDocument);
            _Layers.Insert(0, "No Filter");
        }

    }
}
