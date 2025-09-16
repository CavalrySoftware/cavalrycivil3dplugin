using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Gis.Map.Constants;
using CavalryCivil3DPlugin._ACADLibrary.Elements;
using CavalryCivil3DPlugin.ACADLibrary.Elements;
using CavalryCivil3DPlugin.ACADLibrary.Selection;
using CavalryCivil3DPlugin.C3DLibrary.Selection;

namespace CavalryCivil3DPlugin.CavalryPlugins.LowerPipe.Models
{
    public class ObjectReferenceModel
    {

		private Document _AutocadDocument;

		private string _ReferenceName;
		public string ReferenceName
		{
			get { return _ReferenceName;}
		}


        private (Point3d, Point3d) _EndPoints3d;
        public (Point3d, Point3d) EndPoints3d
        {
            get { return _EndPoints3d; }
        }


		private ObjectId _ObjectId = ObjectId.Null;
        public ObjectId ObjectId_ { get { return _ObjectId; } }


        private double _Cover;
		public double Cover
		{
			get { return _Cover; }
			set { _Cover = value; }
		}


		private double _Depth;
		public double Depth
		{
			get { return _Depth; }
			set { _Depth = value; }
		}


		private PressurePipeModel _PressurePipeReference;
		public PressurePipeModel PressurePipeReference => _PressurePipeReference;


		public ObjectReferenceModel(string _referenceName, Document _autocadDocument)
		{
			_ReferenceName = _referenceName;
			_AutocadDocument = _autocadDocument;
			_PressurePipeReference = new PressurePipeModel(_autocadDocument);
		}

		private bool _EndOfPipe = false;
		public bool EndofPipe => _EndOfPipe;


		public void SetObject()
		{
			switch (ReferenceName)
			{
				case "Polyline":
					SetPolylineReference();
					break;

				case "Pressure Pipe":
					SetPressurePipeReference();
                    break;

				case "Line":
					SetLineReference();
					break ;

				case "End of Pipe Run":
					SetPressurePipeReference();
                    _EndOfPipe = true;
					break;
			}
		}


		private void SetPolylineReference()
		{
            (_ObjectId, _EndPoints3d) = _Polyline.GetSegment(_AutocadDocument);
		}


		private void SetLineReference()
		{
			_ObjectId = ACADObjectSelection.PickLine(_AutocadDocument);
		}


		private void SetPressurePipeReference()
		{
            _ObjectId = C3DObjectSelection.PickPressurePipe(_AutocadDocument, "Reference Pressure Pipe");
			_PressurePipeReference.SetPipe(_ObjectId);
        }


	}
}
