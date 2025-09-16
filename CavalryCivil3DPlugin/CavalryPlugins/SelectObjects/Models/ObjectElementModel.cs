using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace CavalryCivil3DPlugin.CavalryPlugins.SelectObjects.Models
{
    public class ObjectElementModel
    {
		private Type _ElementType;
        public Type ElementType;

        private ObjectId _ObjectId;
        public ObjectId ObjectId; 

        private string _Field;
        public string Field => _Field;

        private string _Value;
        public string Value => _Value;  
	}
}
