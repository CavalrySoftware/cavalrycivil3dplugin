using System;
using System.Collections.Generic;
using System.Windows.Documents;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CavalryCivil3DPlugin.ACADLibrary._ObjectData;
using CavalryCivil3DPlugin.ACADLibrary.Elements;
using CavalryCivil3DPlugin.Consoles;
using PolylineGeometry = Autodesk.AutoCAD.GraphicsInterface.Polyline;

namespace CavalryCivil3DPlugin.Models.CADObject
{
    public class PolylineModel
    {

        #region << PROPERTIES >>
        public readonly Document AutocadDocument;


        private ObjectId _Id;

		public ObjectId Id
		{
			get { return _Id; }
			set { _Id = value; }
		}

        private string Name_;
		public string Name { get { return Name_;}}

        private int Index_;
        public int Index
        {
            get { return Index_; }
            set { Index_ = value; SetNames(sorted: true);} 
        }

        private string LayerName_;

        public string LayerName
        {
            get { return LayerName_; }
        }

        private string _ObjectDataValue;

        public string ObjectDataValue
        {
            get { return _ObjectDataValue; }
        }

        private int _TotalVertices;


        #endregion


        #region << CONSTRUCTOR >>
        public PolylineModel(ObjectId _id, Document _autocadDocument, int _index, string _objectDataValue = null)
		{
			_Id = _id;
            Index_ = _index;
            _ObjectDataValue = _objectDataValue;
            AutocadDocument = _autocadDocument;
            SetNames();
            Test();
        }
        #endregion


        #region << FUNCTIONS >>
        private void SetNames(bool sorted=false)
        {
            string index_string = (Index_ < 10) ? $"0{Index_}" : Index_.ToString();

            if (_ObjectDataValue == null)
            {
                if (sorted)
                {
                    Name_ = $"[{index_string}] Layer: {LayerName_}  -  Total Points: {_TotalVertices}";
                }
                else 
                {
                    using (Transaction tr = AutocadDocument.Database.TransactionManager.StartTransaction())
                    {
                        Polyline polyLine = tr.GetObject(_Id, OpenMode.ForRead) as Polyline;
                        Entity polyLineEntity = polyLine;
                        LayerName_ = polyLineEntity.Layer;
                        int length = (int)polyLine.Length;
                        _TotalVertices = polyLine.NumberOfVertices;
                        string vertices_string = (_TotalVertices < 10) ? $"0{_TotalVertices}" : _TotalVertices.ToString();

                        Name_ = $"[{index_string}] Layer: {LayerName_}  -  Total Points: {vertices_string}";
                    }
                }
            }

            else
            {
                Name_ = $"[{index_string}] {_ObjectDataValue}";
            }
        }


        private void SetNames(int _length, int _vertices, string _layerName)
        {

            LayerName_ = _layerName;
            string vertices_string = (_vertices < 10) ? $"0{_vertices}" : _vertices.ToString();
            string index_string = (Index_ < 10) ? $"0{Index_}" : Index_.ToString();
            Name_ = $"[{index_string}] Total Points: {vertices_string}  -  Total Length: {_length}";
        }


        public int GetNumberOfVertices()
        {
            int vertices;
            using (Transaction tr = AutocadDocument.Database.TransactionManager.StartTransaction())
            {
                Polyline polyLine = tr.GetObject(_Id, OpenMode.ForRead) as Polyline;
                vertices = polyLine.NumberOfVertices;
            }

            return vertices;
        }


        public List<Point3d> GetAllVertices()
        {
            return _Polyline.GetAllVertices(AutocadDocument, _Id);
        }


        private void Test()
        {
            try
            {
                _ObjectDataTable.GetObjectDataValuesfromPolyline(AutocadDocument, _Id);
            }

            catch {}
            
        }
        #endregion

    }
}
