using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.Civil.ApplicationServices;
using CavalryCivil3DPlugin._Library._C3DLibrary._PropertySet;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.SelectObjects.Models
{
    public class SelectObjectsMainModel
    {
        #region CAD DEPENDENCIES
        private Document _AutocadDocument = Application.DocumentManager.MdiActiveDocument;
        private CivilDocument _CivilDocument = CivilApplication.ActiveDocument;
        public DocumentLock MainDocumentLock;
        #endregion


        #region MODEL PROPERTIES
        private ObjectCollectionModel _ObjectCollectionModel;
        public ObjectCollectionModel ObjectCollectionModel => _ObjectCollectionModel;

        private PropertyTypeCollection _PropertyCollection;
        public PropertyTypeCollection PropertyCollection => _PropertyCollection;

        private LayerCollection _LayerCollection;
        public LayerCollection LayerCollection => _LayerCollection;
        #endregion


        #region CONSTRUCTOR
        public SelectObjectsMainModel()
        {
            _ObjectCollectionModel = new ObjectCollectionModel(_AutocadDocument, _CivilDocument);
            _PropertyCollection = new PropertyTypeCollection(_AutocadDocument);
            _LayerCollection = new LayerCollection(_AutocadDocument);
        }
        #endregion
    }
}
