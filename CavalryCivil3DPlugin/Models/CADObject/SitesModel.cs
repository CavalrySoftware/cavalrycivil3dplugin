using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using CavalryCivil3DPlugin.C3DLibrary.Selection;

namespace CavalryCivil3DPlugin.Models.CADObject
{
    public class SitesModel
    {
        #region << GLOBAL VARIABLES >>
        private Document AutocadDocument_;
        #endregion


        #region

        private List<string> _SiteNames;
        public List<string> SiteNames
        {
            get { return _SiteNames; }
        }

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public List<string> Keys { get { return _SiteNames; } }
        #endregion


        public SitesModel(Document _autocadDocument, CivilDocument _civilDocument)
        {
            AutocadDocument_ = _autocadDocument;
            _Name = "Site";
            _SiteNames = C3DObjectSelection.GetAllSiteNames(_autocadDocument, _civilDocument);
            _SiteNames.Sort();
        }
    }
}
