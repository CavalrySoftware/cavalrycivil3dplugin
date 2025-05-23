using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CavalryCivil3DPlugin.ACADLibrary.Elements
{
     class _Polyline
    {

        public static List<Point3d> GetAllVertices(Document _autocadDocument, ObjectId _polylineId)
        {
            List<Point3d> vertices = new List<Point3d>();

            using (Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
            {
                Polyline polyline = tr.GetObject(_polylineId, OpenMode.ForRead) as Polyline;

                for (int i = 0; i < polyline.NumberOfVertices; i++)
                {
                    vertices.Add(polyline.GetPoint3dAt(i));
                }
            }

            return vertices;
        }
    }
}
