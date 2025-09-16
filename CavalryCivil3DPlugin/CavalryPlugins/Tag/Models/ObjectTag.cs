using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.Tag.Models
{
    public class ObjectTag
    {
        private string _Name;
        public string Name => _Name;

        private Type _ObjectType;
        public Type ObjectType => _ObjectType;  

        private CavalryObjectKeyPlanTagCollection _MainTagCollection;

        public ObjectTag(string _name, Type _objectType, CavalryObjectKeyPlanTagCollection _mainTagCollection)
        {
            _Name = _name;
            _ObjectType = _objectType;
            _MainTagCollection = _mainTagCollection;
        }

        public List<CavalryObjectKeyPlanTag> AvailableTagTypes
        {
            get
            {
                if (_ObjectType == typeof(Alignment))
                {
                    _MainTagCollection.SetAvailableTags(_ObjectType);
                    return _MainTagCollection.AlignmentTags;
                }

                else if (_ObjectType == typeof(PressurePart))
                {
                    _MainTagCollection.SetAvailableTags(_ObjectType);
                    return _MainTagCollection.FittingAppurtenanceTags;
                }

                else if (_ObjectType == typeof(Point))
                {
                    _MainTagCollection.SetAvailableTags(_ObjectType);
                    return _MainTagCollection.PointTags;
                }

                else if (_ObjectType == typeof(CogoPoint))
                {
                    _MainTagCollection.SetAvailableTags(_ObjectType);
                    return _MainTagCollection.COGOPointTags;
                }

                else if (_ObjectType == typeof(Polyline))
                {
                    _MainTagCollection.SetAvailableTags(_ObjectType);
                    return _MainTagCollection.PolylineTags;
                }

                return new List<CavalryObjectKeyPlanTag>();
            }
        }
    }
}
