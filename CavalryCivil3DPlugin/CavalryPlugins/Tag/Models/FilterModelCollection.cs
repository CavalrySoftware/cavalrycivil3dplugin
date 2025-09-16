using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;

namespace CavalryCivil3DPlugin.CavalryPlugins.Tag.Models
{
    public class FilterModelCollection
    {
        private List<FilterModel> _Filters = new List<FilterModel>(); 
        public List<FilterModel> Filters => _Filters;

        private Document _AutocadDocument;

        public FilterModelCollection(Document _autocadDocument)
        {
            _AutocadDocument = _autocadDocument;

            InitializeFilters();
        }

        private void InitializeFilters()
        {
            FilterModel noLayer = new FilterModel("None", _AutocadDocument);
            FilterModel filterLayer = new FilterModel("Layer", _AutocadDocument);
            _Filters.Add(noLayer);
            _Filters.Add(filterLayer);
        }
    }
}
