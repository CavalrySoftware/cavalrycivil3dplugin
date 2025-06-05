using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using CavalryCivil3DPlugin._C3DLibrary.Elements;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Models
{
    public class PressurePipeModel
    {

		private double _OuterDiameter;
		public double OuterDiameter
		{
			get { return _OuterDiameter; }
		}


		private ObjectId _ObjectId;
		public ObjectId ObjectId_
		{
			get { return _ObjectId; }
		}


        private ObjectId _PipeRunProfileId;
        public ObjectId PipeRunProfileId
        {
            get { return _ObjectId; }
        }


        private ObjectId _PipeRunAlignmentId;
        public ObjectId PipeRunAlignmentId
        {
            get { return _PipeRunAlignmentId; }
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


		private Document AutocadDocument;


		public PressurePipeModel(ObjectId _objectId, Document _autocadDocument)
		{
			_ObjectId = _objectId;
			AutocadDocument = _autocadDocument;
            _PipeRunProfileId = _PressurePipe.GetProfileId(_objectId, AutocadDocument);
            _PipeRunAlignmentId = _PressurePipe.GetAlignmentId(_objectId, AutocadDocument);
			SetInformations();
        }

		private void SetInformations()
		{
			using (Transaction tr = AutocadDocument.Database.TransactionManager.StartTransaction())
			{
				Alignment alignment = tr.GetObject(_PipeRunAlignmentId, OpenMode.ForRead) as Alignment;
				PressurePipe pressurePipe = tr.GetObject(_ObjectId, OpenMode.ForRead) as PressurePipe;

				_PipeRunName = alignment.Name;
				_NetworkName = pressurePipe.NetworkName;
				_OuterDiameter = pressurePipe.OuterDiameter;
			}
		}

	}
}
