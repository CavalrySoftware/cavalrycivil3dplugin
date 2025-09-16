using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;

namespace CavalryCivil3DPlugin.CavalryPlugins.SelectObjects.Models
{
    public class ObjectModel
    {

        private Type _ObjectType;
        public Type ObjectType => _ObjectType;

        private string _Name;
        public string Name => _Name;

        private List<ObjectId> objectIds;

        public ObjectModel(Type objectType)
        {
            if (objectType == typeof(Alignment))
            {
                _Name = "Alignment";
            }

            else if (objectType == typeof(Polyline))
            {
                _Name = "Polyline";
            }

            else if (objectType == typeof(DBPoint))
            {
                _Name = "Point";
            }

            _ObjectType = objectType;
        }
    }
}
