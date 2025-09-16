using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using CavalryCivil3DPlugin.ACADLibrary.Selection;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.Tag.Models
{
    public class FilterModel
    {
		private string _Name;
		public string Name => _Name;

		private List<string> _FilterKeys;
		public List<string> FilterKeys => _FilterKeys;


		private Document _AutocadDocument;

		public FilterModel(string _name, Document _autocadDocument)
		{
			_Name = _name;
			_AutocadDocument = _autocadDocument;

			GetAvailableKeys();
		}


		private void GetAvailableKeys()
		{
			switch (_Name)
			{
				case "Layer":
					SetLayerKeys();
                    break;

				case "None":
					_FilterKeys = new List<string>() {"None"};
					break;
			}
		}


		private void SetLayerKeys()
		{
            _FilterKeys = ACADObjectSelection.GetAllLayerNames(_AutocadDocument);
		}
	}
}
