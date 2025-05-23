using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using CavalryCivil3DPlugin.ACADLibrary.Selection;

namespace CavalryCivil3DPlugin.Models.CADObject
{
    public class LayersModel
    {
        #region << GLOBAL VARIABLES >>
        private Document AutocadDocument_;
        #endregion


        #region

		private List<string> _LayerNames;
		public List<string> LayerNames
		{
			get { return _LayerNames; }
		}

		private string _Name;

		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		public List<string> Keys { get { return _LayerNames; } }
		#endregion


		public LayersModel(Document _autocadDocument)
		{
			AutocadDocument_ = _autocadDocument;
			_Name = "Layer";
			_LayerNames = ACADObjectSelection.GetAllLayerNames(AutocadDocument_);
		}

	}
}
