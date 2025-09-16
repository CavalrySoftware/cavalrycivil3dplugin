using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using CavalryCivil3DPlugin._Library._C3DLibrary._PropertySet;
using CavalryCivil3DPlugin.ACADLibrary._ObjectData;

namespace CavalryCivil3DPlugin.CavalryPlugins.SelectObjects.Models
{
    public class PropertyTypeModel
    {
        private Document _AutocadDocument;

        private string _Name;
        public string Name => _Name;

        private List<PropertyModel> _Properties = new List<PropertyModel>();
        public List<PropertyModel> Properties => _Properties;

        public PropertyTypeModel(string _propertyType, Document _autocadDocument)
        {
            _Name = _propertyType;
            _AutocadDocument = _autocadDocument;

            switch ( _propertyType )
            {
                case "Property Set":
                    GetPropertySets();
                    break;


                case "Object Data":
                    GetObjectDataTables();
                    break;

                case "No Filter":
                    SetNoFilter();
                    break;
            }
        }


        private void GetPropertySets()
        {
            Dictionary<string, List<string>> propertySets = C3DPropertySet.GetAllPropertySets(_AutocadDocument);

            foreach( string propertySet in propertySets.Keys )
            {
                PropertyModel property = new PropertyModel(propertySet, "Property Set", _AutocadDocument);
                property.Fields = propertySets[propertySet];
                _Properties.Add(property);
            }
        }


        private void GetObjectDataTables()
        {
            Dictionary<string, List<string>> odTables = _ObjectDataTable.GetAllFields( _AutocadDocument );
            foreach(string tableName in odTables.Keys )
            {
                PropertyModel property = new PropertyModel(tableName, "OD Table", _AutocadDocument);
                property.Fields = odTables[tableName];
                _Properties.Add(property);
            }
        }


        private void SetNoFilter()
        {
            PropertyModel property = new PropertyModel("No Filter", "No Filter", _AutocadDocument);
            property.Fields.Add("No Filter");
            _Properties.Add(property);
        }
    }
}
