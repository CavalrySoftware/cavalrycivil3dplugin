using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil;
using Autodesk.Civil.ApplicationServices;
using CavalryCivil3DPlugin.ACADLibrary.AutoCADTable;
using CavalryCivil3DPlugin.ACADLibrary.Selection;
using CavalryCivil3DPlugin.C3DLibrary.Elements;
using CavalryCivil3DPlugin.Consoles;
using CavalryCivil3DPlugin.Models.CADObject;
using DocumentFormat.OpenXml.Drawing;
using CSurface = Autodesk.Civil.DatabaseServices.Surface;

namespace CavalryCivil3DPlugin.CavalryPlugins.ExtractSOP.Models
{
    public class SOPMainModel
    {
        #region CAD DEPENDENCIES
        private Document _AutocadDocument = Application.DocumentManager.MdiActiveDocument;
        private CivilDocument _CivilDocument = CivilApplication.ActiveDocument;
        #endregion


        #region MODEL PROPERTIES
        private SurfaceModelCollection _SurfaceModelCollection;
        public SurfaceModelCollection SurfaceModelCollection_ => _SurfaceModelCollection;

        private TableStyleModel _TableStyleModel;
        public TableStyleModel TableStyleModel_ => _TableStyleModel;

        private PolylineModel _SelectedPolyline;
        public PolylineModel SelectedPolyline
        {
            get { return _SelectedPolyline; }
            set { _SelectedPolyline = value; }
        }

        private List<SOPObjectModel> _SOPs = new List<SOPObjectModel>();
        public List<SOPObjectModel> SOPs
        {
            get { return _SOPs; }
            set { _SOPs = value; }
        }

        private PolylineReferenceModel _PolylineReferenceModel;
        public PolylineReferenceModel PolylineReferenceModel => _PolylineReferenceModel;
        #endregion


        public SOPMainModel()
		{
			_SurfaceModelCollection = new SurfaceModelCollection(_AutocadDocument, _CivilDocument);
			_TableStyleModel = new TableStyleModel(_AutocadDocument);
            _PolylineReferenceModel = new PolylineReferenceModel(_AutocadDocument, _CivilDocument);
		}


        public void SetSop(SurfaceModel _designSuface, SurfaceModel _exsitingSurface)
        {
            _SOPs.Clear();
            if (_PolylineReferenceModel.ValidPolylineReference)
            {
                using (Transaction tr = _AutocadDocument.Database.TransactionManager.StartTransaction())
                {

                int index = 1;
                foreach (Point3d point in _PolylineReferenceModel.PolylineCoordinates)
                    {
                        SOPObjectModel sopObject = new SOPObjectModel(point, index)
                        {
                            DesignElevation = null,
                            ExistingElevation = null,
                        };

                        _SOPs.Add(sopObject);
                        index++;
                    }

                    if (_designSuface.Id != ObjectId.Null)
                    {
                        CSurface designSurface = tr.GetObject(_designSuface.Id, OpenMode.ForRead) as CSurface;
                        foreach(SOPObjectModel sopObject in _SOPs)
                        {
                            try
                            {
                                sopObject.DesignElevation = designSurface.FindElevationAtXY(sopObject.X, sopObject.Y);
                            }

                            catch { }
                        }
                    }

                    if (_exsitingSurface.Id != ObjectId.Null)
                    {
                        CSurface existingSurface = tr.GetObject(_exsitingSurface.Id, OpenMode.ForRead) as CSurface;
                        foreach (SOPObjectModel sopObject in _SOPs)
                        {
                            try
                            {
                                sopObject.ExistingElevation = existingSurface.FindElevationAtXY(sopObject.X, sopObject.Y);
                            }

                            catch { }
                        }
                    }

                }
            }
        }


        public List<List<string>> GetSOPData()
        {
            List<List<string >> sopData = new List<List<string>>();

            foreach(SOPObjectModel sop in  _SOPs)
            {
                List<string> rowData = new List<string>();

                rowData.Add(sop.PointName);
                rowData.Add(sop.X.ToString("F3"));
                rowData.Add(sop.Y.ToString("F3"));
                rowData.Add(sop.ExistingElevation.HasValue ? sop.ExistingElevation.Value.ToString("F3") : string.Empty);
                rowData.Add(sop.DesignElevation.HasValue ? sop.DesignElevation.Value.ToString("F3") : string.Empty);
                sopData.Add(rowData);
            }

            return sopData;
        }


        public void CreateTable(string _tableStyle=null, bool _annotate=false, string _prefix=null)
        {
            if (_SOPs.Count > 0)
            {
                (Point3d insertionPoint, bool validPoint) = ACADObjectSelection.PickInsertionPoint(_AutocadDocument);

                if (validPoint)
                {
                    List<List<string>> allData = GetSOPData();
                    List<string> headers = new List<string>()
                {
                    "REF",
                    "EASTING",
                    "NORTHING",
                    "EXISTING ELEVATION",
                    "DESIGN ELEVATION"
                };
                    string title = "SOP";

                    CADTable.CreateTable(_AutocadDocument, allData, insertionPoint, _headers: headers, _title: title, _tableStyle: _tableStyle);

                    if (_annotate)
                    {
                        AnnotatePoint(_prefix);   
                    }
                }
            }
        }

        private void AnnotatePoint(string _prefix=null)
        {
            using (DocumentLock docLock = _AutocadDocument.LockDocument())
            {
                using (Transaction tr = _AutocadDocument.Database.TransactionManager.StartTransaction())
                {
                    BlockTable blockTable = tr.GetObject(_AutocadDocument.Database.BlockTableId, OpenMode.ForWrite) as BlockTable;
                    BlockTableRecord blockTableRecord = tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    double xOffset = 1;
                    double yOffset = 1;

                    foreach (SOPObjectModel sop in _SOPs)
                    {
                        DBText text = new DBText();
                        text.Position = new Point3d(sop.X + xOffset, sop.Y + yOffset, 0);
                        text.Height = 1.5;
                        text.TextString = string.IsNullOrEmpty(_prefix) ? $"{sop.PointName}" : _prefix + sop.PointName.Substring(1);
                        blockTableRecord.AppendEntity(text);
                        tr.AddNewlyCreatedDBObject(text, true);
                    }
                    tr.Commit();
                }
            }
        }
	}
}
