using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;

namespace CavalryCivil3DPlugin.CavalryPlugins.SelectObjects.Models
{
    public class PropertyModel
    {
		private string _Name;
		private Document _AutocadDocument;

		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}


		private List<string> _Fields = new List<string>();
		public List<string> Fields 
		{ 
			get 
			{ 
				return _Fields;
			} 
			set 
			{ 
				_Fields = value; 
			}
		}


		public PropertyModel(string _name, string _propType, Document _autocadDocument)
		{
			switch (_propType)
			{
				case "Property Set":
					break;
				case "OD Table":
					break ;
			}

			_Name = _name;
		}


		private void GetPropertySetFields()
		{

		}


		private void GetODSetFields()
		{

		}
	}
}
