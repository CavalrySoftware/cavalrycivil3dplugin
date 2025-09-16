using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;

namespace CavalryCivil3DPlugin.CavalryPlugins.SelectObjects.Models
{
    public class PropertyTypeCollection
    {

        private List<PropertyTypeModel> _PropertyTypeModels = new List<PropertyTypeModel>();
        public List<PropertyTypeModel> PropertyTypeModels => _PropertyTypeModels;

        private Document _AutocadDocument;

        public PropertyTypeCollection(Document _autocadDocument)
        {
            _AutocadDocument = _autocadDocument;

            PropertyTypeModel noFilterType = new PropertyTypeModel(_propertyType: "No Filter", _AutocadDocument);
            PropertyTypeModel propertySet = new PropertyTypeModel(_propertyType: "Property Set", _AutocadDocument);
            PropertyTypeModel objectData = new PropertyTypeModel(_propertyType: "Object Data", _AutocadDocument);

            _PropertyTypeModels.Add(noFilterType);
            _PropertyTypeModels.Add(propertySet);
            _PropertyTypeModels.Add(objectData);
        }
    }
}
