using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace CavalryCivil3DPlugin.Models.CADObject
{
    public class SurfaceModel
    {

		private ObjectId _Id;

		public ObjectId Id
		{
			get { return _Id; }
			set { _Id = value; }
		}

		private string _Name;
		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}


	}
}
