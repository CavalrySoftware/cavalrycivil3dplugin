using System;
using System.Collections.Generic;
using System.Windows.Documents;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using CavalryCivil3DPlugin.ACADLibrary._ObjectData;
using CavalryCivil3DPlugin.ACADLibrary.Elements;
using CavalryCivil3DPlugin.Consoles;
using PolylineGeometry = Autodesk.AutoCAD.GraphicsInterface.Polyline;
using CADEntity = Autodesk.AutoCAD.DatabaseServices.Entity;
using CavalryCivil3DPlugin.C3DLibrary.Elements;

namespace CavalryCivil3DPlugin.Models.CADObject
{
    public class FeatureLinesModel
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
        public string Name { get { return Name_; } }

        private int Index_;

        public int Index
        {
            get { return Index_; }
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


        #endregion


        #region << CONSTRUCTOR >>
        public FeatureLinesModel(ObjectId _id, Document _autocadDocument, int _index, string _objectDataValue = null)
        {
            _Id = _id;
            Index_ = _index;
            _ObjectDataValue = _objectDataValue;
            AutocadDocument = _autocadDocument;
            SetNames();
            //Test();
        }
        #endregion


        #region << FUNCTIONS >>
        private void SetNamesr()
        {
            string index_string = (Index_ < 10) ? $"0{Index_}" : Index_.ToString();

            if (_ObjectDataValue == null)
            {
                using (Transaction tr = AutocadDocument.Database.TransactionManager.StartTransaction())
                {
                    Polyline polyLine = tr.GetObject(_Id, OpenMode.ForRead) as Polyline;
                    CADEntity polyLineEntity = polyLine;
                    LayerName_ = polyLineEntity.Layer;

                    int length = (int)polyLine.Length;
                    var vertices = polyLine.NumberOfVertices;
                    string vertices_string = (vertices < 10) ? $"0{vertices}" : vertices.ToString();

                    Name_ = $"[{index_string}] Total Points: {vertices_string}  -  Total Length: {length}";
                }
            }

            else
            {
                Name_ = $"[{index_string}] {_ObjectDataValue}";
            }
        }

        private void SetNames()
        {

            using (Transaction tr = AutocadDocument.Database.TransactionManager.StartTransaction())
            {
                FeatureLine entity = tr.GetObject(_Id, OpenMode.ForRead) as FeatureLine;
                Name_ = $"[{Index_}] {entity.Name}";
            }
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
            return _FeatureLines.GetPIPoints(AutocadDocument, _Id);
        }


        private void Test()
        {
            try
            {
                _ObjectDataTable.GetObjectDataValuesfromPolyline(AutocadDocument, _Id);
            }

            catch { }

        }
        #endregion

    }
}
