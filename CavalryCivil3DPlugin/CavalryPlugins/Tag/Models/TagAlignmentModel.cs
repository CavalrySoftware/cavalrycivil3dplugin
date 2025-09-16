using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using CavalryCivil3DPlugin.C3DLibrary.Selection;
using CavalryCivil3DPlugin.CavalryPlugins.Tag.ViewModels;
using CavalryCivil3DPlugin.Models.CADObject;

namespace CavalryCivil3DPlugin.CavalryPlugins.Tag.Models
{
    public class TagAlignmentModel
    {
        private AlignmentModelCollection _AlignmentModelCollection;
        public AlignmentModelCollection AlignmentModelCollection => _AlignmentModelCollection;

        private ObjectId _SingleAlignmentTagId;
        public ObjectId SingleAlignmentTagId => _SingleAlignmentTagId;

        private Point3d _PickedPoint;
        public Point3d PickedPoint => _PickedPoint;

        #region CAD DEPENDENCIES
        private Document _AutocadDocument;
        private CivilDocument _CivilDocument;
        #endregion

        #region ASSOCIATED OBJECTS DEFINITION
        private TagMainViewModel _TagMainViewModel;
        #endregion


        #region CONSTRUCTOR
        public TagAlignmentModel(Document _autocadDocument, CivilDocument _civilDocument, TagMainViewModel _tagMainViewModel)
        {
            _AutocadDocument = _autocadDocument;
            _CivilDocument = _civilDocument;
            _TagMainViewModel = _tagMainViewModel;

            InitializeObject();
        }
        #endregion


        #region INITIALIZE
        private void InitializeObject()
        {
            _AlignmentModelCollection = new AlignmentModelCollection(_AutocadDocument, _CivilDocument);
        }
        #endregion


        #region OPERATION FUNCTIONS

        public void TagAlignmentExecute()
        {
            switch (_TagMainViewModel.SelectedMethod)
            {
                case TagMainViewModel.NotificationMethod.Single:
                    SetSingleAlignment();
                    if (_SingleAlignmentTagId != ObjectId.Null) TagSingleAlignment();
                    break;
            }
        }


        private void SetSingleAlignment()
        {
            (_SingleAlignmentTagId, _PickedPoint) = C3DObjectSelection.PickAlignmentWithPoint(_AutocadDocument);
        }


        private void TagSingleAlignment()
        {
            using(Transaction tr = _AutocadDocument.Database.TransactionManager.StartTransaction())
            {
                Alignment alignment = tr.GetObject(_SingleAlignmentTagId, OpenMode.ForRead) as Alignment;
                //ObjectId labelStyleId = _TagMainViewModel.SelectedTag.Id;

                double station = 0.0;
                double offset = 0.0;
                alignment.StationOffset(_PickedPoint.X, _PickedPoint.Y, ref station, ref offset);
            }
        }

        #endregion
    }
}
