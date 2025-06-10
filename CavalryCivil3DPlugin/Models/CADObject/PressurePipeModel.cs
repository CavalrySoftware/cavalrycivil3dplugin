using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.Settings;
using CavalryCivil3DPlugin._C3DLibrary.Elements;
using CavalryCivil3DPlugin.Consoles;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Models
{
    public class PressurePipeModel
    {

        #region << OBJECT ID PROPERTIES >>
        private ObjectId _ObjectId = ObjectId.Null;
        public ObjectId ObjectId_
        {
            get { return _ObjectId; }
        }


        private ObjectId _RunProfileId = ObjectId.Null;
        public ObjectId RunProfileId
        {
            get { return _RunProfileId; }
        }


        private ObjectId _RunAlignmentId = ObjectId.Null;
        public ObjectId RunAlignmentId
        {
            get { return _RunAlignmentId; }
        }

        #endregion


        #region << INFORMATION PROPERTIES >>
        private double _OuterDiameter;
		public double OuterDiameter
		{
			get { return _OuterDiameter; }
		}

		private string _PipeRunName;

		public string PipeRunName
		{
			get { return _PipeRunName; }
			set { _PipeRunName = value; }
		}

		private string _NetworkName;

		public string NetworkName
		{
			get { return _NetworkName; }
			set { _NetworkName = value; }
		}
        #endregion


        private Document AutocadDocument;


		public PressurePipeModel(Document _autocadDocument)
		{
			AutocadDocument = _autocadDocument;
        }


		public void SetPipe(ObjectId _objectId)
		{
			_ObjectId = _objectId;

			if(!_ObjectId.IsNull)
			{
				SetInformations();
			}
		}


		private void SetInformations()
		{
			using (Transaction tr = AutocadDocument.Database.TransactionManager.StartTransaction())
			{
                _RunProfileId = _PressurePipe.GetProfileId(_ObjectId, AutocadDocument);
                _RunAlignmentId = _PressurePipe.GetAlignmentId(_ObjectId, AutocadDocument);
                Alignment alignment = tr.GetObject(_RunAlignmentId, OpenMode.ForRead) as Alignment;
				PressurePipe pressurePipe = tr.GetObject(_ObjectId, OpenMode.ForRead) as PressurePipe;

				_PipeRunName = alignment.Name;
				_NetworkName = pressurePipe.NetworkName;
				_OuterDiameter = pressurePipe.OuterDiameter;
			}
		}
	}
}
