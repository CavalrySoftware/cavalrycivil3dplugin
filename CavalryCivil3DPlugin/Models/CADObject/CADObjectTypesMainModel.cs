using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.Civil.ApplicationServices;
using CavalryCivil3DPlugin.Models.CADObject;

namespace CavalryCivil3DPlugin.Models
{
    public class CADObjectTypesMainModel
    {

        private Document AutoCADDocument;
        private CivilDocument Civil3DDocument;

        #region Properties
        private List<CADObjectModel> CADObjectTypes_ = new List<CADObjectModel>(); 

        public List<CADObjectModel> CADObjectTypes
        {
            get
            {
                return CADObjectTypes_;
            }
            set 
            {
                CADObjectTypes_ = value;
            }
        }

        private List<LayersModel> ExistingLayers_;
        public List<LayersModel> ExistingLayers
        {
            get { return ExistingLayers_; }
            set { ExistingLayers_ = value; }
        }


        #endregion


        public CADObjectTypesMainModel(Document _document, List<string> _cadObjects, CivilDocument _civilDocument)
        {
            AutoCADDocument = _document;
            Civil3DDocument = _civilDocument;

            foreach (var objectType in _cadObjects)
            {
                CADObjectTypes_.Add(new CADObjectModel(objectType, AutoCADDocument, Civil3DDocument));
            }
        }


        private void LoadLayers()
        {

        }
    }
}
