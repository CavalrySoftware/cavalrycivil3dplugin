using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CavalryCivil3DPlugin.ACADLibrary.Selection;
using CavalryCivil3DPlugin.Consoles;

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


        public static (ObjectId, (Point3d, Point3d)) GetSegment(Document _autocadDocument)
        {
            (ObjectId polylineId, Point3d pickedPoint ) = ACADObjectSelection.PickPolylineGetPoint(_autocadDocument);


            using(Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
            {
                Polyline polyline = tr.GetObject(polylineId, OpenMode.ForRead) as Polyline;
                var totalVertices = polyline.NumberOfVertices;
                var totalSegments = totalVertices - 1;
                double value = 0;
                
                Point3d plinePoint = polyline.GetClosestPointTo(pickedPoint, false);

                for (int i = 0; i < totalSegments; i++)
                {
                    if (polyline.OnSegmentAt(i, new Point2d(plinePoint.X, plinePoint.Y), value ))
                    {
                        LineSegment3d segment =  polyline.GetLineSegmentAt(i);
                        return (polylineId, (segment.StartPoint, segment.EndPoint));
                    }
                }

                return (polylineId, (new Point3d(0, 0 ,0), new Point3d(0, 0 , 0)));

            }
        }

        public static Point3d GetMidPoint(Document _autocadDocument, ObjectId _polylineId)
            
        {
            using (Transaction tr = _autocadDocument.Database.TransactionManager.StartTransaction())
            {
                Polyline polyline = tr.GetObject(_polylineId, OpenMode.ForRead) as Polyline;    
                double halfLength = polyline.Length / 2.0;

                double param = polyline.GetParameterAtDistance(halfLength);

                return polyline.GetPointAtParameter(param);
            }
        }
    }
}
