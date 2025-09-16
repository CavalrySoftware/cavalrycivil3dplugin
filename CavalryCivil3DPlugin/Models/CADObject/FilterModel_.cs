using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CavalryCivil3DPlugin.Models.CADObject
{
    public class FilterModel_
    {

        #region << PROPERTIES >>
        private string _FilterName;

		public string FilterName
		{
			get { return _FilterName; }
			set { _FilterName = value; }
		}

		private dynamic _FilterObject;
		public dynamic FilterObject { get { return _FilterObject; } }
		#endregion


		private List<string> _FilterOptions;

		public List<string> FilterOptions
		{
			get { return _FilterOptions; }
		}


		#region << CONSTRUCTOR >>
		public FilterModel_(dynamic filterObject)
		{
			_FilterObject = filterObject;
            _FilterName = filterObject.Name;
			_FilterOptions = filterObject.Keys;
        }

        #endregion

    }
}
