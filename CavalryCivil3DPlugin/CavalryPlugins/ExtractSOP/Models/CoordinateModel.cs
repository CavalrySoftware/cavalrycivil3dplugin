using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Geometry;

namespace CavalryCivil3DPlugin.CavalryPlugins.ExtractSOP.Models
{
    public class CoordinateModel
    {
		private Point3d _Cooordinates;

		public Point3d Coordinates { 
			get { return _Cooordinates; }
			set { _Cooordinates = value; }
		}

		private string _Name;

		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}
	}
}
