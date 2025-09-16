using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.Civil.ApplicationServices;
using CavalryCivil3DPlugin.CavalryPlugins.Tag.ViewModels;
using CavalryCivil3DPlugin.Models.CADObject;

namespace CavalryCivil3DPlugin.CavalryPlugins.Tag.Models
{

    public class TagMainModel
    {
        #region CAD DEPENDENCIES
        private Document _AutocadDocument = Application.DocumentManager.MdiActiveDocument;
        private CivilDocument _CivilDocument = CivilApplication.ActiveDocument;
        public DocumentLock MainDocumentLock;
        #endregion


        #region MODEL PROPERTIES DEFINITION
        private CavalryObjectKeyPlanTagCollection _KeyPlanTagCollection;
        public CavalryObjectKeyPlanTagCollection KeyPlanTagCollection => _KeyPlanTagCollection;


        private FilterModelCollection _FilterModelCollection;
        public FilterModelCollection FilterModelCollection => _FilterModelCollection;
        #endregion


        #region ASSOCIATED OBJECTS DEFINITIONS
        private TagMainViewModel _MainViewModel;
        public TagMainViewModel MainViewModel => _MainViewModel;
        #endregion


        #region CONSTRUCTOR
        public TagMainModel (TagMainViewModel mainViewModel)
        {
            MainDocumentLock = _AutocadDocument.LockDocument();
            _MainViewModel = mainViewModel;

            InitializeModelProperties();
        }
        #endregion


        #region INITIALIZE MODEL PROPERTIES
        private void InitializeModelProperties()
        {
            _KeyPlanTagCollection = new CavalryObjectKeyPlanTagCollection(_AutocadDocument, _CivilDocument);
            _FilterModelCollection = new FilterModelCollection(_AutocadDocument);
        }
        #endregion
    }
}
