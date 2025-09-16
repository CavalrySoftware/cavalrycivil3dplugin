using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using CavalryCivil3DPlugin.ACADLibrary.AutoCADTable;

namespace CavalryCivil3DPlugin.Models.CADObject
{
    public class TableStyleModel
    {

        private List<string> _ExistingStyleNames;

        public List<string> ExistingStyleNames
        {
            get { return _ExistingStyleNames; }
        }

        private Document _AutocadDocument;


        private int _DefaultItem;
        public int DefaultItem
        {
            get { return _DefaultItem; }
        }

        private string _DefaultStyle;
        public string DefaultStyle => _DefaultStyle;



        private string _DefaultStyleName = "Coordinates Easting Northing";

        public TableStyleModel(Document _autocadDocument)
        {
            _AutocadDocument = _autocadDocument;
            _ExistingStyleNames = CADTable.GetAllTableStyleNames(_AutocadDocument);
            SetDefault();
        }

        private void SetDefault()
        {
            _DefaultItem = _ExistingStyleNames.Contains(_DefaultStyleName) ? _ExistingStyleNames.IndexOf(_DefaultStyleName) : 0;
            _DefaultStyle = _ExistingStyleNames[_DefaultItem];
        }
    }
}
